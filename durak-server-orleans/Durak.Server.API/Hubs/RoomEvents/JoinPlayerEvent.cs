using Durak.Server.API.Models;

namespace Durak.Server.API.Hubs.RoomEvents;

public class JoinPlayerEvent(RoomPlayer roomPlayer) : IEvent
{
    public string MethodName => "JoinPlayer";

    public RoomPlayer RoomPlayer { get; set; } = roomPlayer;
}
