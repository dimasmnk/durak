namespace Durak.Server.API.Models;

[GenerateSerializer]
public class PlayerState(long id, bool isPassed, bool isWantToTake, int cardCount)
{
    [Id(0)]
    public long Id { get; set; } = id;
    [Id(1)]
    public bool IsPassed { get; set; } = isPassed;
    [Id(2)]
    public bool IsWantToTake { get; set; } = isWantToTake;
    [Id(3)]
    public int CardCount { get; set; } = cardCount;
}
