using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NockChat.Models.Sessions;
using NockChat.Services.Common.Exceptions;
using NockChat.Services.Common.Extensions;

namespace NockChat.Services.Common.DataStorage.Sessions
{
    /// <summary>
    /// Реализация локального хранилища сессий на основе JSON-файла
    /// </summary>
    public class LocalSessionService(ILogger<LocalSessionService> logger) : ILocalSessionService
    {
        private readonly string _filePath = Path.Combine(AppPaths.DataFolder, "sessions.json");

        /// <summary>
        /// Настройки сериализации JSON
        /// Кэшируется статически, чтобы избежать повторной инициализации и рефлексии при каждом вызове
        /// </summary>
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        /// <inheritdoc/>
        public async Task SaveAsync(RoomSession session, CancellationToken ct = default)
        {
            try
            {
                var sessions = await LoadAllAsync(ct);
                var roomId = ParseRoomId(session.Token);

                sessions.RemoveAll(s => ParseRoomId(s.Token) == roomId);
                sessions.Add(session);

                await WriteAsync(sessions, ct);
            }
            catch (StorageException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Не удалось сохранить сессию");
                throw new StorageException("Не удалось сохранить сессию", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<List<RoomSession>> LoadAllAsync(CancellationToken ct = default)
        {
            if (!File.Exists(_filePath))
                return [];

            try
            {
                var json = await File.ReadAllTextAsync(_filePath, ct);
                return JsonSerializer.Deserialize<List<RoomSession>>(json, JsonOptions) ?? [];
            }
            catch (Exception ex)
            {
                throw new StorageException("Не удалось загрузить сессии", ex);
            }
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string token, CancellationToken ct = default)
        {
            try
            {
                var sessions = await LoadAllAsync(ct);
                var roomId = ParseRoomId(token);

                sessions.RemoveAll(s => ParseRoomId(s.Token) == roomId);
                await WriteAsync(sessions, ct);
            }
            catch (StorageException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new StorageException("Не удалось удалить сессию", ex);
            }
        }

        /// <summary>
        /// Сериализует список сессий и записывает его в файл
        /// </summary>
        private async Task WriteAsync(List<RoomSession> sessions, CancellationToken ct)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
                var json = JsonSerializer.Serialize(sessions, JsonOptions);

                await File.WriteAllTextAsync(_filePath, json, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Не удалось записать файл сессий");
                throw new StorageException("Не удалось записать файл сессий", ex);
            }
        }

        /// <summary>
        /// Извлекает идентификатор комнаты из JWT-токена
        /// Возвращает <c>-1</c> если токен невалиден или не содержит claim <c>roomId</c>
        /// </summary>
        private static int ParseRoomId(string token)
        {
            try
            {
                var payload = token.Split('.')[1];
                var padded = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
                var json = Encoding.UTF8.GetString(Convert.FromBase64String(padded));
                var claims = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!;

                return int.Parse(claims["roomId"].GetString()!);
            }
            catch
            {
                return -1;
            }
        }
    }
}