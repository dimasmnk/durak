using Durak.Core.RoomModule.Enums;

namespace Durak.Core.RoomModule.Entities;

public class Player
{
    private Player(long id, string firstName)
    {
        Id = id;
        FirstName = firstName;
        Status = PlayerStatus.NotReady;
    }

    public long Id { get; private set; }
    public string FirstName { get; set; }
    public PlayerStatus Status { get; private set; }

    public static Player CreatePlayer(long playerId, string firstName)
    {
        return new Player(playerId, firstName);
    }

    public void SetReady()
    {
        Status = PlayerStatus.Ready;
    }
}