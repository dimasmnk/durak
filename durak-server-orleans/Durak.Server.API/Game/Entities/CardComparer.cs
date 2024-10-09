using Durak.Server.API.Game.Enums;

namespace Durak.Server.API.Game.Entities;

public class CardComparer(Suit trump) : IComparer<Card>
{
    private readonly Suit _trump = trump;

    public int Compare(Card? x, Card? y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);

        if (x.Suit == y.Suit)
        {
            return x.Rank.CompareTo(y.Rank);
        }

        if (x.Suit == _trump && y.Suit != _trump)
        {
            return 1;
        }

        if (y.Suit == _trump && x.Suit != _trump)
        {
            return -1;
        }

        throw new ArgumentException("Cards must be of the same suit or one of them must be trump");
    }
}
