using Durak.Server.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Durak.Server.API.Context;
public interface IDurakDbContext
{
    DbSet<Player> Players { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}