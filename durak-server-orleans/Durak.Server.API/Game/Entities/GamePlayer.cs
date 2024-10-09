namespace Durak.Server.API.Game.Entities;

public class GamePlayer(long id)
{
    public long Id { get; set; } = id;
    public List<Card> Cards { get; set; } = [];
    public bool IsPassed { get; set; }
    public bool IsWantToTake { get; set; }

    public void Reset()
    {
        IsPassed = false;
        IsWantToTake = false;
    }
}
