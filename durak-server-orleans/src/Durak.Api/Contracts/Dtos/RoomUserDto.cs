namespace Durak.Api.Contracts.Dtos;

public class RoomUserDto
{
    public long UserId { get; set; }
    public bool IsReady { get; set; }
    public string Username { get; set; } = string.Empty;
}
