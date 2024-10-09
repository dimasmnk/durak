using Durak.Api.Entities;
using Durak.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Durak.Api.Repositories;

public class RoomUserRepository(IDurakDbContext context) : IRoomUserRepository
{
    private readonly IDurakDbContext _context = context;

    public async Task<bool> IsRoomUserExistsAsync(long userId, CancellationToken cancellationToken)
    {
        return await _context.RoomUsers.AnyAsync(ru => ru.UserId == userId, cancellationToken);
    }

    public async Task AddRoomUser(RoomUser roomUser, CancellationToken cancellationToken)
    {
        await _context.RoomUsers.AddAsync(roomUser, cancellationToken);
    }
}
