using Durak.Api.Entities;
using Durak.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Durak.Api.Repositories;

public class RoomSettingRepository(IDurakDbContext context) : IRoomSettingRepository
{
    private readonly IDurakDbContext _context = context;

    public async Task<RoomSetting> GetRoomSettingNoTrackingAsync(Guid roomId, CancellationToken cancellationToken)
    {
        return await _context.RoomSettings
            .AsNoTracking()
            .FirstAsync(rs => rs.Id == roomId, cancellationToken);
    }
}
