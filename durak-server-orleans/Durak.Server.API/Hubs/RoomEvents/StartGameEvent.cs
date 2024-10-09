using Durak.Server.API.Models;

namespace Durak.Server.API.Hubs.RoomEvents;

public class StartGameEvent(GameState gameState) : IEvent
{
    public string MethodName => "StartGame";
    public GameState GameState { get; set; } = gameState;
}
