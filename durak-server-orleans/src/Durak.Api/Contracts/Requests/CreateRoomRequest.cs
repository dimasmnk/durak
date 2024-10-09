namespace Durak.Api.Contracts.Requests;

public class CreateRoomRequest
{
    public int Bet { get; set; }
    public bool IsPublic { get; set; }
    public byte MaxPlayerCount { get; set; }
}
