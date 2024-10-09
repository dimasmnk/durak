using Durak.Server.API.Context;
using Durak.Server.API.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Durak.Server.API.Repositories;

public class PlayerRepository(IDurakDbContext durakDbContext) : IPlayerRepository
{
    private readonly IDurakDbContext _durakDbContext = durakDbContext;

    public ValueTask<Player?> GetPlayerByIdAsync(long id, CancellationToken cancellationToken)
    {
        return _durakDbContext.Players.FindAsync(new object[] { id }, cancellationToken);
    }

    public ValueTask<EntityEntry<Player>> CreatePlayerAsync(Player player, CancellationToken cancellationToken)
    {
        return _durakDbContext.Players.AddAsync(player, cancellationToken);
    }

    public void UpdatePlayer(Player player)
    {
        _durakDbContext.Players.Update(player);
    }
}
