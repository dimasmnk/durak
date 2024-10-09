namespace Durak.Api.Hubs.RoomListEvents;

public class RoomRemovalEventData(Guid roomId) : IEvent
{
    public string MethodName { get; } = "RemoveRoom";

    public Guid RoomId { get; set; } = roomId;
}
