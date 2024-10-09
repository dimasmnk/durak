namespace Durak.Api.Entities;

public class RoomUser
{
    public long UserId { get; set; }
    public Guid RoomId { get; set; }
    public bool IsReady { get; set; }

    public User User { get; set; } = null!;
    public Room Room { get; set; } = null!;
}
