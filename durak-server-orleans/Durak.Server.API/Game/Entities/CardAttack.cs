namespace Durak.Server.API.Game.Entities;

[GenerateSerializer]
public class CardAttack(Card attackCard)
{
    [Id(0)]
    public Card AttackCard { get; set; } = attackCard;
    [Id(1)]
    public Card? DefendCard { get; set; }
}
