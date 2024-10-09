using Durak.Server.API.Models;

namespace Durak.Server.API.Hubs.RoomEvents;

public class SyncGameStateEvent(GameState gameState) : IEvent
{
    public string MethodName => "SyncGameState";

    public GameState GameState { get; set; } = gameState;
}
