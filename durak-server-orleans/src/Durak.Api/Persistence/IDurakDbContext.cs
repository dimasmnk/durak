using Durak.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Durak.Api.Persistence;

public interface IDurakDbContext
{
    DbSet<User> Users { get; }
    DbSet<Room> Rooms { get; }
    DbSet<RoomSetting> RoomSettings { get; }
    DbSet<RoomUser> RoomUsers { get; }

    DatabaseFacade Database { get; }
    DbSet<PublicRoom> PublicRooms { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}