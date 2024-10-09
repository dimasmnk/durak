using Durak.Core.GameModule.Constants;
using Durak.Core.GameModule.Enums;
using Durak.Core.GameModule.Exceptions;
using Durak.Core.GameModule.Helpers;
using Durak.Core.GameModule.ValueObjects;

namespace Durak.Core.GameModule.Entities;

public class Game
{
    private readonly CardComparer _cardComparer;
    private readonly Random _random = new();

    private Game(Guid id, Dictionary<int, long> playerIds)
    {
        Id = id;
        Trump = Enum.GetValues<Suit>().MinBy(_ => _random.Next());
        _cardComparer = new CardComparer(Trump);
        Players = ConvertToPlayers(playerIds.Values.ToList());
        Deck = new Stack<Card>(GameConstants.DefaultDeck.OrderBy(_ => _random.Next()));
        DealCards();
        Turn = GetFirstTurn();
        UpdateTickAndUpdatedAt();
    }

    public Guid Id { get; set; }
    public int Tick { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Suit Trump { get; set; }
    public Stack<Card> Deck { get; set; }

    public Turn Turn { get; set; }

    public List<Player> Players { get; set; }
    public List<long> WinnerIds { get; set; } = [];
    public Player Defender => Players.Single(x => x.Id == Turn.DefenderId);
    public Player Attacker => Players.Single(x => x.Id == Turn.AttackerId);

    public static Game CreateGame(Guid id, Dictionary<int, long> playerIds)
    {
        return new Game(id, playerIds);
    }

    public void Attack(long playerId, Card card)
    {
        ValidatePlayerInGame(playerId);
        ValidatePlayerAttacker(playerId);
        ValidateDefenderHasCards();
        var player = Players.Single(x => x.Id == playerId);

        Turn.Attack(player.DrawCard(card));

        ResetPlayerStatuses();
        Attacker.SetPassStatus();
        TryMovePlayerToWinnerIfEmpty(playerId);
        UpdateTickAndUpdatedAt();
    }

    public void Defend(long playerId, int attackCardId, Card card)
    {
        ValidatePlayerInGame(playerId);
        ValidatePlayerDefender(playerId);
        var player = Players.Single(x => x.Id == playerId);

        Turn.Defend(attackCardId, player.DrawCard(card));

        ResetPlayerStatuses();
        Defender.SetPassStatus();
        TryMovePlayerToWinnerIfEmpty(playerId);
        UpdateTickAndUpdatedAt();
    }

    public void Pass(long playerId)
    {
        ValidatePlayerInGame(playerId);
        var player = Players.Single(x => x.Id == playerId);
        if (playerId != Turn.DefenderId) player.SetPassStatus();
        if (playerId == Turn.DefenderId) throw new Exception("Defender cannot set pass.");
        TrySetNextTurn();
    }

    public void Take(long playerId)
    {
        ValidatePlayerInGame(playerId);
        var player = Players.Single(x => x.Id == playerId);
        if (playerId != Turn.DefenderId) throw new Exception("Only defending players can pass.");
        player.SetTakeStatus();
        TrySetNextTurn();
    }

    public bool IsGameFinished()
    {
        var deckIsEmpty = Deck.Count == 0;
        var playerWithCardsCount = Players.Count(x => x.Hand.Count > 0) <= 1;

        return deckIsEmpty && playerWithCardsCount;
    }

    public GameResult GetGameResult()
    {
        var isDraw = Players.Count == 0;
        return new GameResult { WinnerIds = WinnerIds, IsDraw = isDraw };
    }

    private void TrySetNextTurn()
    {
        var areAllPlayersWithStatuses = Players.Where(x => x.Hand.Count > 0).All(x => x.Status != PlayerStatus.None);
        if (areAllPlayersWithStatuses) SetNextTurn();
    }

    private void SetNextTurn()
    {
        TakeCardsForDefenderIfIsTakeStatus();
        DealCards();
        var isTake = Defender.IsTake;
        var currentAttackPlaceId = Players.IndexOf(Attacker);
        RemoveEmptyPlayers();
        if (IsGameFinished()) return;
        var nextTurnPlayerOffset = isTake ? 2 : 1;
        var nextAttackPlaceId = (currentAttackPlaceId + nextTurnPlayerOffset) % Players.Count;
        var nextDefensePlaceId = (nextAttackPlaceId + 1) % Players.Count;
        var attackPlayerId = Players[nextAttackPlaceId].Id;
        var defensePlayerId = Players[nextDefensePlaceId].Id;
        Turn = new Turn(attackPlayerId, defensePlayerId, [], _cardComparer);
        ResetPlayerStatuses();
        UpdateTickAndUpdatedAt();
    }

    private void TryMovePlayerToWinnerIfEmpty(long playerId)
    {
        if (Deck.Count != 0) return;
        var player = Players.Single(x => x.Id == playerId);
        if (player.Hand.Count != 0) return;
        WinnerIds.Add(playerId);
    }

    private void RemoveEmptyPlayers()
    {
        Players.RemoveAll(x => WinnerIds.Contains(x.Id));
    }

    private void TakeCardsForDefenderIfIsTakeStatus()
    {
        if (Turn.IsAllCardsBeaten || !Defender.IsTake) return;

        var tableCards = Turn.CardPairs.Values.SelectMany(x => new[] { x.AttackCard, x.DefenseCard })
            .Where(x => x is not null)
            .ToList();

        Defender.Hand.AddRange(tableCards!);
    }

    private void ResetPlayerStatuses()
    {
        foreach (var player in Players) player.ResetStatus();
    }

    private void DealCards()
    {
        if (Deck.Count == 0) return;

        foreach (var player in Players)
        {
            var playerCardCount = player.Hand.Count;
            var needToAddCount = GameConstants.HandCardCount - playerCardCount;
            if (needToAddCount <= 0) continue;
            foreach (var _ in Enumerable.Range(0, needToAddCount))
            {
                if (Deck.Count == 0) return;
                var card = Deck.Pop();
                player.Hand.Add(card);
            }
        }
    }

    private Turn GetFirstTurn()
    {
        int? playerPlaceId = null;
        Card? lowestTrumpCard = null;

        foreach (var player in Players)
        {
            var playerLowestTrumpCard = player.Hand.Where(x => x.Suit == Trump)
                .MinBy(x => x.Rank);

            if (playerLowestTrumpCard == null) continue;

            if (lowestTrumpCard == null)
            {
                lowestTrumpCard = playerLowestTrumpCard;
                continue;
            }

            if (playerLowestTrumpCard.Rank <= lowestTrumpCard.Rank) continue;
            lowestTrumpCard = playerLowestTrumpCard;
            playerPlaceId = Players.IndexOf(player);
        }

        playerPlaceId ??= _random.Next(1, Players.Count);

        var attackPlaceId = playerPlaceId.Value;
        var defensePlaceId = (playerPlaceId.Value + 1) % Players.Count;

        var attackPlayerId = Players[attackPlaceId].Id;
        var defensePlayerId = Players[defensePlaceId].Id;

        return new Turn(attackPlayerId, defensePlayerId, [], _cardComparer);
    }

    private static List<Player> ConvertToPlayers(List<long> playerIds)
    {
        return playerIds.Select(x => new Player { Id = x }).ToList();
    }

    private void UpdateTickAndUpdatedAt()
    {
        Tick++;
        UpdatedAt = DateTime.UtcNow;
    }

    private void ValidatePlayerInGame(long playerId)
    {
        var isPlayerExistInGame = Players.Any(x => x.Id == playerId);
        if (!isPlayerExistInGame) throw new PlayerNotInGameException(Id, playerId);
    }

    private void ValidatePlayerAttacker(long playerId)
    {
        var isPlayerAttackerIfFirstAttack = Turn.CardPairs.Count == 0 && Turn.AttackerId == playerId;
        var isPlayerThrowInAndNotDefender = Turn.CardPairs.Count != 0 && Turn.DefenderId != playerId;
        var canAttack = isPlayerAttackerIfFirstAttack || isPlayerThrowInAndNotDefender;

        if (!canAttack) throw new PlayerIsNotAttackerException(playerId);
    }

    private void ValidatePlayerDefender(long playerId)
    {
        if (playerId != Turn.DefenderId)
            throw new PlayerIsNotDefenderException(playerId);
    }

    private void ValidateDefenderHasCards()
    {
        if (Defender.Hand.Count == 0)
            throw new Exception("Defender has no cards to defend.");
    }
}