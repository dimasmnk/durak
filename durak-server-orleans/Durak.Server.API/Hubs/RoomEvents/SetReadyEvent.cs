namespace Durak.Server.API.Hubs.RoomEvents;

public class SetReadyEvent(long playerId) : IEvent
{
    public string MethodName { get; } = "SetReady";

    public long PlayerId { get; set; } = playerId;
}
