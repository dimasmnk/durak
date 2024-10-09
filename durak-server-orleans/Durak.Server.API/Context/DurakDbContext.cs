using Durak.Server.API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Durak.Server.API.Context;

public class DurakDbContext(DbContextOptions options) : DbContext(options), IDurakDbContext
{
    public DbSet<Player> Players => Set<Player>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
