using Durak.Server.API.Entities;
using Durak.Server.API.Game.Entities;

namespace Durak.Server.API.Services.Interfaces;

public interface IPlayerService
{
    Task<Player> GetOrCreatePlayerAsnyc(long id, CancellationToken cancellationToken);
    Task UpdatePlayerAsync(Player player, CancellationToken cancellationToken);
    ValueTask<Player> GetPlayerAsync(long playerId);
    Task<bool> TryJoinRoomAsync(long playerId, string roomId, CancellationToken cancellationToken);
    Task<bool> TryLeaveRoomAsync(long playerId);
    Task SetPlayerNameAsync(long playerId, string name);
    ValueTask<string> GetCurrentRoomId(long playerId);
    Task SetReadyAsync(long playerId);
    ValueTask AttackAsync(long playerId, Card card);
    ValueTask DefendAsync(long playerId, int cardAttackId, Card card);
    ValueTask SetPassAsync(long playerId);
    ValueTask SetWantToTakeAsync(long playerId);
    ValueTask<bool> TryWithdrawAsync(long playerId, long coinCount);
    Task DepositAsync(long playerId, long coinCount);
    Task UpdatePlayerStatisticAsync(long playerId, bool isWin);
}