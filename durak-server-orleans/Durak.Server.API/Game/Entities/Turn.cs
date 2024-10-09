namespace Durak.Server.API.Game.Entities;

[GenerateSerializer]
public class Turn
{
    [Id(0)]
    public long AttackerId { get; set; }
    [Id(1)]
    public long DefenderId { get; set; }
    [Id(2)]
    public List<CardAttack> TableCards { get; set; } = [];

    public void Reset()
    {
        TableCards.Clear();
    }
}
