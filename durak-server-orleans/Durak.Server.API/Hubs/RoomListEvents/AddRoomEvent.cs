using Durak.Server.API.Enums;
using Durak.Server.API.Models;

namespace Durak.Server.API.Hubs.RoomListEvents;

public class AddRoomEvent(string connectionId, RoomSettings roomSettings, byte playerCount) : IEvent
{
    public string MethodName { get; } = "AddRoom";

    public string ConnectionId { get; set; } = connectionId;
    public Guid RoomId { get; set; } = roomSettings.Id;
    public Bet Bet { get; set; } = roomSettings.Bet;
    public byte PlayerCount { get; set; } = playerCount;
}
