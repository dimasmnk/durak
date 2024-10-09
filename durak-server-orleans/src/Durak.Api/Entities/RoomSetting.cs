namespace Durak.Api.Entities;

public class RoomSetting
{
    public Guid Id { get; set; }
    public int Bet { get; set; }
    public bool IsPublic { get; set; }
    public int MaxPlayerCount { get; set; }
    public Room Room { get; set; } = null!;
}
