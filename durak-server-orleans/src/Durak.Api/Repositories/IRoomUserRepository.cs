using Durak.Api.Entities;

namespace Durak.Api.Repositories;
public interface IRoomUserRepository
{
    Task AddRoomUser(RoomUser roomUser, CancellationToken cancellationToken);
    Task<bool> IsRoomUserExistsAsync(long userId, CancellationToken cancellationToken);
}