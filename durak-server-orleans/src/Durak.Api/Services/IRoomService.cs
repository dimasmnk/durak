using Durak.Api.Contracts.Dtos;
using Durak.Api.Contracts.Requests;

namespace Durak.Api.Services;
public interface IRoomService
{
    Task<Guid> CreateRoomAsync(CreateRoomRequest createRoomRequest, long userId, CancellationToken cancellationToken);
    Task<List<RoomInListDto>> GetPublicRoomsAsync(CancellationToken cancellationToken);
    Task<RoomDto> GetRoomByIdAsync(Guid roomId, CancellationToken cancellationToken);
    Task<bool> TryJoinRoomAsync(long userId, Guid roomId, CancellationToken cancellationToken);
}