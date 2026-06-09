using System.Text.Json;
using NockChat.Application.Common.Interfaces;
using NockChat.Domain.Entities;
using StackExchange.Redis;

namespace NockChat.Infrastructure.Redis
{
    /// <summary>
    /// Redis-реализация <see cref="IParticipantKeyRepository"/>
    /// Ключи хранятся только в памяти — автоматически исчезают по TTL или удаляются при корректном отключении участника
    /// </summary>
    public class RedisParticipantKeyRepository(IConnectionMultiplexer redis) : IParticipantKeyRepository
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// TTL ключа участника, страховка на случай если клиент упал без корректного отключения —
        /// ключ автоматически исчезнет через 24 часа
        /// </summary>
        private static readonly TimeSpan KeyTtl = TimeSpan.FromHours(24);

        private readonly IDatabase _db = redis.GetDatabase();

        /// <inheritdoc/>
        /// <remarks>
        /// Выполняет два Redis-запроса:
        /// SETEX — записывает JSON участника с TTL,
        /// SADD — добавляет chatUserId в SET комнаты.
        /// При повторном вызове SETEX перезаписывает старый ключ — участник перезашёл с новым keypair
        /// </remarks>
        public async Task UpsertAsync(ParticipantKey key, CancellationToken ct = default)
        {
            var dto = new ParticipantKeyDto
            {
                ChatUserId = key.ChatUserId,
                RoomId = key.RoomId,
                Username = key.ChatUser?.Username ?? string.Empty,
                EphemeralPublicKey = key.EphemeralPublicKey,
                PublishedAt = key.PublishedAt
            };

            var json = JsonSerializer.Serialize(dto, JsonOptions);

            await _db.StringSetAsync(KeyFor(key.RoomId, key.ChatUserId), json, KeyTtl);
            await _db.SetAddAsync(RoomSetKey(key.RoomId), key.ChatUserId);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Выполняет два Redis-запроса:
        /// SMEMBERS — получает все chatUserId комнаты за один round-trip,
        /// MGET — батчевый GET всех ключей участников за один round-trip.
        /// Итого два обращения к Redis независимо от числа участников
        /// </remarks>
        public async Task<IReadOnlyList<ParticipantKey>> GetRoomKeysAsync(
            int roomId, int excludeChatUserId, CancellationToken ct = default)
        {
            // Получаем все chatUserId участников комнаты за один запрос
            var memberIds = await _db.SetMembersAsync(RoomSetKey(roomId));

            if (memberIds.Length == 0)
                return [];

            // Исключаем себя и формируем массив Redis-ключей для батчевого GET
            var redisKeys = memberIds.Where(id => (int)id != excludeChatUserId).Select(id => (RedisKey)KeyFor(roomId, (int)id)).ToArray();

            if (redisKeys.Length == 0)
                return [];

            // Батчевый GET — один round-trip для всех участников
            var values = await _db.StringGetAsync(redisKeys);

            var result = new List<ParticipantKey>();
            foreach (var value in values)
            {
                // Ключ мог исчезнуть по TTL между SMEMBERS и MGET — пропускаем
                if (value.IsNullOrEmpty)
                    continue;

                var dto = JsonSerializer.Deserialize<ParticipantKeyDto>((string)value!, JsonOptions);
                if (dto is null)
                    continue;

                // Восстанавливаем навигационное свойство ChatUser —
                // необходимо для совместимости с интерфейсом и маппинга в RoomKeyResponse
                result.Add(new ParticipantKey
                {
                    ChatUserId = dto.ChatUserId,
                    RoomId = dto.RoomId,
                    EphemeralPublicKey = dto.EphemeralPublicKey,
                    PublishedAt = dto.PublishedAt,
                    ChatUser = new ChatUser
                    {
                        Id = dto.ChatUserId,
                        Username = dto.Username
                    }
                });
            }

            return result;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Выполняет два Redis-запроса:
        /// DEL  — удаляет данные участника,
        /// SREM — удаляет chatUserId из SET комнаты.
        /// Вызывается в OnDisconnectedAsync — ключ исчезает сразу при отключении,
        /// не дожидаясь истечения TTL
        /// </remarks>
        public async Task DeleteAsync(int chatUserId, int roomId, CancellationToken ct = default)
        {
            await _db.KeyDeleteAsync(KeyFor(roomId, chatUserId));
            await _db.SetRemoveAsync(RoomSetKey(roomId), chatUserId);
        }

        /// <summary>
        /// Формирует Redis-ключ для хранения данных конкретного участника комнаты.
        /// Формат: <c>participant_key:{roomId}:{chatUserId}</c>
        /// </summary>
        private static string KeyFor(int roomId, int chatUserId)
            => $"participant_key:{roomId}:{chatUserId}";

        /// <summary>
        /// Формирует Redis-ключ для SET участников комнаты.
        /// Формат: <c>participant_keys:{roomId}</c>
        /// </summary>
        private static string RoomSetKey(int roomId)
            => $"participant_keys:{roomId}";
    }
}