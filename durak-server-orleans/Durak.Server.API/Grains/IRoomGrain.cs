using Durak.Server.API.Game.Entities;
using Durak.Server.API.Models;

namespace Durak.Server.API.Grains;

public interface IRoomGrain : IGrainWithStringKey
{
    ValueTask AttackAsync(long playerId, Card card);
    ValueTask DefendAsync(long playerId, int cardAttackId, Card card);
    ValueTask<GameState> GetGameStateAsync(long playerId);
    ValueTask<RoomState> GetRoomStateAsync();
    Task SetOnlineStatusAsync(long playerId, bool isConnected);
    ValueTask SetPassAsync(long playerId);
    Task SetReadyAsync(long playerId);
    ValueTask SetWantToTakeAsync(long playerId);
    Task<bool> TryAddPlayerAsync(long playerId, string playerName);
    Task<bool> TryRemovePlayerAsync(long playerId);
}
