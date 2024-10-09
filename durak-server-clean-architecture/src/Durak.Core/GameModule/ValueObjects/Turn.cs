using Durak.Core.GameModule.Constants;
using Durak.Core.GameModule.Exceptions;
using Durak.Core.GameModule.Helpers;

namespace Durak.Core.GameModule.ValueObjects;

public record Turn(long AttackerId, long DefenderId, Dictionary<int, CardPair> CardPairs, CardComparer CardComparer)
{
    public bool IsAllCardsBeaten => CardPairs.All(x => x.Value.DefenseCard != null);

    public void Attack(Card card)
    {
        ValidateMaxCardCount();
        ValidateTableHasRankIfNotFirstAttack(card);
        CardPairs.Add(CardPairs.Count, new CardPair(card));
    }

    public void Defend(int attackCardId, Card card)
    {
        ValidateCardStrength(CardPairs[attackCardId].AttackCard, card);
        CardPairs[attackCardId].DefenseCard = card;
    }

    private void ValidateCardStrength(Card attackCard, Card defenseCard)
    {
        if (CardComparer.Compare(attackCard, defenseCard) > 0)
            throw new InsufficientCardStrengthException();
    }

    private void ValidateMaxCardCount()
    {
        if (CardPairs.Count >= GameConstants.TurnMaxCardCount)
            throw new TurnMaxCardCountIsExceededException();
    }

    private void ValidateTableHasRankIfNotFirstAttack(Card card)
    {
        if (CardPairs.Count == 0) return;

        var tableCards = CardPairs.Values.SelectMany(x => new[] { x.AttackCard, x.DefenseCard })
            .Where(x => x is not null)
            .ToList();

        if (tableCards.Count > 0 && tableCards.All(x => x!.Rank != card.Rank))
            throw new CardRankMismatchException();
    }
}