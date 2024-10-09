using Durak.Api.Entities;

namespace Durak.Api.Repositories;
public interface IRoomRepository
{
    Task<Room> AddRoomAsync(Room room, CancellationToken cancellationToken);
    Task<List<Room>> GetPublicRoomsNoTrackingAsync(CancellationToken cancellationToken);
    Task<Room> GetRoomNoTrackingAsync(Guid roomId, CancellationToken cancellationToken);
    Task<bool> IsRoomExistsAsync(Guid roomId, CancellationToken cancellationToken);
    Task UpdateRoomPlayerCountAsync(Guid roomId, int playerCount, CancellationToken cancellationToken);
}