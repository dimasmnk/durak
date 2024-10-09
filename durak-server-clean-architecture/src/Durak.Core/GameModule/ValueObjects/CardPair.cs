namespace Durak.Core.GameModule.ValueObjects;

public record CardPair(Card AttackCard)
{
    public Card? DefenseCard { get; set; }
}