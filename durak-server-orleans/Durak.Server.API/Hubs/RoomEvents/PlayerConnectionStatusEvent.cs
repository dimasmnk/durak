namespace Durak.Server.API.Hubs.RoomEvents;

public class PlayerConnectionStatusEvent(long playerId, bool isConnected) : IEvent
{
    public string MethodName => "PlayerConnectionStatus";

    public long PlayerId { get; set; } = playerId;
    public bool IsConnected { get; set; } = isConnected;
}
