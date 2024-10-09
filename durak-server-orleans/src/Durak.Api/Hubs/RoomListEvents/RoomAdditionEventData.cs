using Durak.Api.Entities;

namespace Durak.Api.Hubs.RoomListEvents;

public class RoomAdditionEventData(RoomSetting roomSetting, int playerCount) : IEvent
{
    public string MethodName { get; } = "AddRoom";

    public Guid RoomId { get; set; } = roomSetting.Id;
    public int Bet { get; set; } = roomSetting.Bet;
    public int MaxPlayerCount { get; set; } = roomSetting.MaxPlayerCount;
    public int PlayerCount { get; set; } = playerCount;
}
