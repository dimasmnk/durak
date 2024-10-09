using Durak.Server.API.Game.Entities;
using Durak.Server.API.Models;

namespace Durak.Server.API.Services.Interfaces;
public interface IRoomService
{
    bool IsPublic { get; }
    string Key { get; }
    Dictionary<long, RoomPlayer> Players { get; }
    RoomSettings RoomSettings { get; }
    bool AreAllReady { get; }
    bool IsGameStarted { get; set; }

    void FinishGame(object sender, GameResult gameResult);
    RoomState GetRoomState();
    Task InitAsync(string key, CancellationToken cancellationToken);
    void Reset();
    void SetReady(long playerId);
    Task Terminate(CancellationToken cancellationToken);
    Task<bool> TryAddPlayerAsync(long playerId, string playerName);
    Task<bool> TryRemovePlayerAsync(long playerId);
}