using Durak.Api.Entities;

namespace Durak.Api.Repositories;
public interface IRoomSettingRepository
{
    Task<RoomSetting> GetRoomSettingNoTrackingAsync(Guid roomId, CancellationToken cancellationToken);
}