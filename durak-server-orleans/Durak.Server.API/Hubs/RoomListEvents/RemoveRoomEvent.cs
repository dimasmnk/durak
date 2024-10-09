namespace Durak.Server.API.Hubs.RoomListEvents;

public class RemoveRoomEvent(string connectionId) : IEvent
{
    public string MethodName { get; } = "RemoveRoom";

    public string ConnectionId { get; set; } = connectionId;
}
