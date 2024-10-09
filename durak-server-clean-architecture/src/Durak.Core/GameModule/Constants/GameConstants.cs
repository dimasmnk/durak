using System.Collections.Immutable;
using Durak.Core.GameModule.Enums;
using Durak.Core.GameModule.ValueObjects;

namespace Durak.Core.GameModule.Constants;

public static class GameConstants
{
    public static readonly ImmutableList<Card> DefaultDeck = Enum.GetValues<Suit>()
        .SelectMany(suit => Enum.GetValues<Rank>().Select(rank => new Card(suit, rank)))
        .ToImmutableList();

    public static int HandCardCount => 6;
    public static int TurnMaxCardCount => 6;
}