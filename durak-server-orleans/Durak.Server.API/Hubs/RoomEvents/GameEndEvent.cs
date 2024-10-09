namespace Durak.Server.API.Hubs.RoomEvents;

public class GameEndEvent(long winnerId) : IEvent
{
    public string MethodName => "GameEnd";
    public long WinnerId { get; set; } = winnerId;
}
