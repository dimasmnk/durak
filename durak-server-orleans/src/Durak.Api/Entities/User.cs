namespace Durak.Api.Entities;

public class User
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int CoinCount { get; set; }
    public int WinCount { get; set; }
    public int LossCount { get; set; }
    public int DrawCount { get; set; }
    public int TotalCount { get; set; }

    public RoomUser RoomUser { get; set; } = null!;
}
