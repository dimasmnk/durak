namespace Durak.Server.API.Models;

[GenerateSerializer]
public class RoomState(List<RoomPlayer> players, bool isGameStarted, RoomSettings roomSettings)
{
    [Id(0)]
    public List<RoomPlayer> Players { get; set; } = players;
    [Id(1)]
    public bool IsGameStarted { get; set; } = isGameStarted;
    [Id(2)]
    public RoomSettings RoomSettings { get; set; } = roomSettings;
}
