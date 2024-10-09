using Durak.Server.API.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Durak.Server.API.Repositories;
public interface IPlayerRepository
{
    ValueTask<Player?> GetPlayerByIdAsync(long id, CancellationToken cancellationToken);
    ValueTask<EntityEntry<Player>> CreatePlayerAsync(Player player, CancellationToken cancellationToken);
    void UpdatePlayer(Player player);
}