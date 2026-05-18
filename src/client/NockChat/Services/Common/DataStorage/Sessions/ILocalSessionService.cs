using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NockChat.Models.Sessions;

namespace NockChat.Services.Common.DataStorage.Sessions
{
    public interface ILocalSessionService
    {
        Task SaveAsync(RoomSession session, CancellationToken ct = default);
        Task<List<RoomSession>> LoadAllAsync(CancellationToken ct = default);
        Task RemoveAsync(string token, CancellationToken ct = default);
    }
}