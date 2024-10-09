using Durak.Server.API.Models;

namespace Durak.Server.API.Services.Interfaces;
public interface IRoomListService
{
    Task AddRoomAsync(string roomName, RoomSettings roomSettings, CancellationToken cancellationToken);
    Task RemoveRoomAsync(string roomName, CancellationToken cancellationToken = default);
    Task UpdateRoomAsync(string roomId, int playerCount, CancellationToken cancellationToken = default);
    string GetRandomAvailableRoomConnectionId();

    ICollection<Room> Rooms { get; }
}