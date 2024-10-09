using Durak.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Durak.Api.Persistence;

public class DurakDbContext(DbContextOptions options) : DbContext(options), IDurakDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<RoomSetting> RoomSettings => Set<RoomSetting>();
    public DbSet<RoomUser> RoomUsers => Set<RoomUser>();
    public DbSet<PublicRoom> PublicRooms => Set<PublicRoom>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
