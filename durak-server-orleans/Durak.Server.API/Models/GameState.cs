using Durak.Server.API.Game.Entities;
using Durak.Server.API.Game.Enums;

namespace Durak.Server.API.Models;

[GenerateSerializer]
public class GameState(
    List<PlayerState> playerStates,
    int deckCardCount,
    Suit trump,
    Card trumpCard,
    Turn currentTurn,
    List<Card> cards)
{
    [Id(0)]
    public List<PlayerState> PlayerStates { get; set; } = playerStates;
    [Id(1)]
    public int DeckCardCount { get; set; } = deckCardCount;
    [Id(2)]
    public Suit Trump { get; set; } = trump;
    [Id(3)]
    public Card TrumpCard { get; set; } = trumpCard;
    [Id(4)]
    public Turn CurrentTurn { get; set; } = currentTurn;
    [Id(5)]
    public List<Card> Cards { get; set; } = cards;
}
