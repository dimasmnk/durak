using Durak.Server.API.Enums;

namespace Durak.Server.API.Models;

public class Room(string connectionId, RoomSettings roomSettings, int playerCount)
{
    public string ConnectionId { get; set; } = connectionId;
    public Guid RoomId { get; set; } = roomSettings.Id;
    public Bet Bet { get; set; } = roomSettings.Bet;
    public int PlayerCount { get; set; } = playerCount;
}
