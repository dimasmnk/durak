namespace Durak.Api.Entities;

public class Room
{
    public Guid Id { get; set; }
    public bool IsGameInProgress { get; set; }
    public int PlayerCount { get; set; }

    public RoomSetting RoomSetting { get; set; } = null!;
    public ICollection<RoomUser> RoomUsers { get; set; } = null!;
    public PublicRoom? PublicRoom { get; set; } = null!;
}
