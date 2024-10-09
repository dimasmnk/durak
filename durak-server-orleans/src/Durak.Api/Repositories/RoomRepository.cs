using Durak.Api.Entities;
using Durak.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Durak.Api.Repositories;

public class RoomRepository(IDurakDbContext context) : IRoomRepository
{
    private readonly IDurakDbContext _context = context;

    public async Task<Room> AddRoomAsync(Room room, CancellationToken cancellationToken)
    {
        await _context.Rooms.AddAsync(room, cancellationToken);
        return room;
    }

    public async Task<Room> GetRoomNoTrackingAsync(Guid roomId, CancellationToken cancellationToken)
    {
        return await _context.Rooms
            .AsNoTracking()
            .Include(r => r.RoomSetting)
            .Include(r => r.RoomUsers)
            .ThenInclude(r => r.User)
            .FirstAsync(r => r.Id == roomId, cancellationToken);
    }

    public async Task<bool> IsRoomExistsAsync(Guid roomId, CancellationToken cancellationToken)
    {
        return await _context.Rooms.AnyAsync(r => r.Id == roomId, cancellationToken);
    }

    public async Task UpdateRoomPlayerCountAsync(Guid roomId, int playerCount, CancellationToken cancellationToken)
    {
        var room = await _context.Rooms.FindAsync([roomId], cancellationToken);
        if (room is not null)
        {
            room.PlayerCount = playerCount;
        }
    }

    public async Task<List<Room>> GetPublicRoomsNoTrackingAsync(CancellationToken cancellationToken)
    {
        return await _context.PublicRooms
            .AsNoTracking()
            .Include(x => x.Room)
            .ThenInclude(x => x.RoomSetting)
            .Select(x => x.Room)
            .ToListAsync(cancellationToken);
    }
}
