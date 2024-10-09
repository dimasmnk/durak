using Durak.Core.GameModule.Enums;
using Durak.Core.GameModule.ValueObjects;

namespace Durak.Core.GameModule.Helpers;

public class CardComparer(Suit trump) : IComparer<Card>
{
    public int Compare(Card? x, Card? y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);

        if (x.Suit == y.Suit) return x.Rank.CompareTo(y.Rank);

        if (x.Suit == trump && y.Suit != trump) return 1;

        if (y.Suit == trump && x.Suit != trump) return -1;

        throw new ArgumentException("Cards must be of the same suit or one of them must be trump");
    }
}