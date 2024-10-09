namespace Durak.Server.API.Hubs.RoomEvents;

public class LeavePlayerEvent(long playerId) : IEvent
{
    public string MethodName => "LeavePlayer";

    public long PlayerId { get; set; } = playerId;
}
