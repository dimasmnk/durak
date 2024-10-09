namespace Durak.Server.API.Hubs.RoomListEvents;

public class UpdateRoomPlayerCountEvent(string connectionId, int playerCount) : IEvent
{
    public string MethodName { get; } = "UpdateRoomPlayerCount";

    public string ConnectionId { get; set; } = connectionId;
    public int PlayerCount { get; set; } = playerCount;
}
