namespace Durak.Api.Hubs.RoomListEvents;

public class RoomPlayerCountUpdateEventData(Guid roomId, int playerCount) : IEvent
{
    public string MethodName { get; } = "UpdateRoomPlayerCount";

    public Guid RoomId { get; set; } = roomId;
    public int PlayerCount { get; set; } = playerCount;
}
