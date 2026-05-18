using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NockChat.Models.Sessions;
using NockChat.Services.Common.Extensions;

namespace NockChat.Services.Common.DataStorage.Sessions
{
    public class LocalSessionService : ILocalSessionService
    {
        private readonly string _filePath = Path.Combine(AppPaths.DataFolder, "sessions.json");

        public async Task SaveAsync(RoomSession session, CancellationToken ct = default)
        {
            var sessions = await LoadAllAsync(ct);

            // Парсим токен чтобы получить roomId для дедупликации
            var roomId = ParseRoomId(session.Token);
            sessions.RemoveAll(s => ParseRoomId(s.Token) == roomId);
            sessions.Add(session);

            await WriteAsync(sessions, ct);
        }

        public async Task<List<RoomSession>> LoadAllAsync(CancellationToken ct = default)
        {
            if (!File.Exists(_filePath)) return [];

            var json = await File.ReadAllTextAsync(_filePath, ct);
            return JsonSerializer.Deserialize<List<RoomSession>>(json) ?? [];
        }

        public async Task RemoveAsync(string token, CancellationToken ct = default)
        {
            var sessions = await LoadAllAsync(ct);
            var roomId = ParseRoomId(token);
            sessions.RemoveAll(s => ParseRoomId(s.Token) == roomId);
            await WriteAsync(sessions, ct);
        }

        private async Task WriteAsync(List<RoomSession> sessions, CancellationToken ct)
        {
            var json = JsonSerializer.Serialize(sessions, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json, ct);
        }

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