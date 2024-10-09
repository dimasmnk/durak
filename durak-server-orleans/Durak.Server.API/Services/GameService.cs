using Durak.Server.API.Game.Constants;
using Durak.Server.API.Game.Entities;
using Durak.Server.API.Game.Enums;
using Durak.Server.API.Hubs.RoomEvents;
using Durak.Server.API.Models;
using Durak.Server.API.Services.Interfaces;

namespace Durak.Server.API.Services;

public class GameService(IRoomEventService roomEventService) : IGameService
{
    private readonly IRoomEventService _roomEventService = roomEventService;
    private readonly Dictionary<long, System.Timers.Timer> playerTimers = [];

    public Stack<Card> Deck { get; set; } = [];
    public Suit Trump => TrumpCard.Suit;
    public Card TrumpCard { get; set; } = new Card();
    public Dictionary<long, GamePlayer> Players { get; set; } = [];
    public List<long> PlayerIds { get; set; } = [];
    public Turn CurrentTurn { get; set; } = new Turn();
    public event EventHandler<GameResult>? GameEnded;

    public async Task StartGameAsync(IEnumerable<long> playerIds)
    {
        Reset();
        CreateDeck();
        SetTrump();
        SetupPlayers(playerIds);
        SetAttackerAndDefender();
        await SendStartGameEventToPlayersAsync();
        SetupTimers();
    }

    public void SetupPlayers(IEnumerable<long> playerIds)
    {
        foreach (var playerId in playerIds)
        {
            Players.Add(playerId, new GamePlayer(playerId));
        }

        PlayerIds = [.. playerIds];

        for (var i = 0; i < GameConstants.HandCardCount; i++)
        {
            foreach (var playerId in playerIds)
            {
                Players[playerId].Cards.Add(Deck.Pop());
            }
        }
    }

    public void DealCards()
    {
        if (Deck.Count <= 0)
            return;

        var attacker = Players[CurrentTurn.AttackerId];

        if (attacker is not null)
        {
            while (attacker.Cards.Count < GameConstants.HandCardCount && Deck.Count > 0)
            {
                DealCard(CurrentTurn.AttackerId);
            }
        }

        foreach (var player in Players.Where(x => x.Key != CurrentTurn.DefenderId && x.Key != CurrentTurn.AttackerId))
        {
            while (player.Value.Cards.Count < GameConstants.HandCardCount && Deck.Count > 0)
            {
                DealCard(player.Key);
            }
        }

        var defender = Players[CurrentTurn.DefenderId];
        if (defender is not null)
        {
            while (defender.Cards.Count < GameConstants.HandCardCount && Deck.Count > 0)
            {
                DealCard(CurrentTurn.DefenderId);
            }
        }
    }

    public void DealCard(long playerId)
    {
        if (Deck.Count == 0) return;

        if (!Players.ContainsKey(playerId))
        {
            throw new ArgumentException("Player does not exist");
        }

        if (Players[playerId].Cards.Count == GameConstants.HandCardCount)
        {
            throw new ArgumentException("Player already has maximum number of cards");
        }

        Players[playerId].Cards.Add(Deck.Pop());
    }

    public void SetAttackerAndDefender()
    {
        var playerId = PlayerIds.First();
        var card = new Card();

        foreach (var player in Players)
        {
            var playerCard = player.Value.Cards
                .OrderBy(x => x.Rank)
                .Where(x => x.Suit == Trump)
                .FirstOrDefault();

            if (playerCard is null)
            {
                continue;
            }

            if (playerCard.Rank < card.Rank)
            {
                playerId = player.Key;
                card = playerCard;
            }
        }

        CurrentTurn.AttackerId = playerId;
        var index = PlayerIds.IndexOf(playerId);
        CurrentTurn.DefenderId = PlayerIds[(index + 1) % PlayerIds.Count];
    }

    private void SetTrump()
    {
        TrumpCard = Deck.Last();
    }

    private void CreateDeck()
    {
        var cards = new List<Card>();

        foreach (var suit in Enum.GetValues<Suit>())
        {
            foreach (var rank in Enum.GetValues<Rank>())
            {
                cards.Add(new Card(rank, suit));
            }
        }

        var random = new Random();
        Deck = new Stack<Card>(cards.OrderBy(x => random.Next()));
    }

    public async Task AttackAsync(long playerId, Card card)
    {
        if (CurrentTurn.TableCards.Count == 0 && playerId != CurrentTurn.AttackerId)
        {
            throw new ArgumentException("Player is not attacker");
        }

        if (!Players[playerId].Cards.Contains(card))
        {
            throw new ArgumentException("Player does not have this card");
        }

        var tableCards = CurrentTurn.TableCards.SelectMany(x => new[] { x.AttackCard, x.DefendCard }).Where(x => x is not null).ToList();

        if (tableCards.Count > 0 && !tableCards.Any(x => x!.Rank == card.Rank))
        {
            throw new ArgumentException("Player cannot attack using this card");
        }

        Players[playerId].Cards.Remove(card);
        CurrentTurn.TableCards.Add(new CardAttack(card));
        ResetPassForAll();
        ResetTimers();
        await SyncStateAsync();
        CheckForGameEnd();
    }

    public async Task DefendAsync(long playerId, int cardAttackId, Card defendCard)
    {
        if (CurrentTurn.TableCards.Count == 0)
        {
            throw new ArgumentException("Table is empty");
        }

        if (playerId != CurrentTurn.DefenderId)
        {
            throw new ArgumentException("Player is not defender");
        }

        if (!Players[playerId].Cards.Contains(defendCard))
        {
            throw new ArgumentException("Player does not have this card");
        }

        if (CurrentTurn.TableCards[cardAttackId].DefendCard is not null)
        {
            throw new ArgumentException("Card is already defended");
        }

        var attackCard = CurrentTurn.TableCards[cardAttackId].AttackCard;
        var comparer = new CardComparer(Trump);
        var result = comparer.Compare(attackCard, defendCard);

        if (result > 0)
        {
            throw new ArgumentException("Card is not strong enough");
        }

        Players[playerId].Cards.Remove(defendCard);
        CurrentTurn.TableCards[cardAttackId].DefendCard = defendCard;
        ResetPassForAll();
        ResetTimers();
        await SyncStateAsync();
        CheckForGameEnd();
    }

    public void Take(long playerId)
    {
        if (CurrentTurn.TableCards.Count == 0)
        {
            throw new ArgumentException("Table is empty");
        }

        if (playerId != CurrentTurn.DefenderId)
        {
            throw new ArgumentException("Player is not defender");
        }

        var cards = CurrentTurn.TableCards.Select(x => x.AttackCard).ToList();
        cards.AddRange(CurrentTurn.TableCards.Where(x => x.DefendCard is not null).Select(x => x.DefendCard!));
        Players[playerId].Cards.AddRange(cards);
        CurrentTurn.TableCards.Clear();
    }

    public async Task NextTurnAsync()
    {
        var isWantToTake = Players[CurrentTurn.DefenderId].IsWantToTake;

        CurrentTurn.Reset();

        foreach (var player in Players)
        {
            player.Value.Reset();
        }

        DealCards();

        var offset = isWantToTake ? 1 : 0;
        var defenderId = PlayerIds.IndexOf(CurrentTurn.DefenderId);
        CurrentTurn.AttackerId = PlayerIds[(defenderId + offset) % PlayerIds.Count];
        CurrentTurn.DefenderId = PlayerIds[(defenderId + offset + 1) % PlayerIds.Count];

        await SyncStateAsync();
        ResetTimers();
    }

    public async Task SetPassAsync(long playerId)
    {
        Players[playerId].IsPassed = true;

        playerTimers[playerId].Stop();
        await SyncStateAsync();
        await CheckForNextTurnAsync();
    }

    public async Task SetWantToTakeAsync(long playerId)
    {
        if (playerId != CurrentTurn.DefenderId)
        {
            throw new ArgumentException("Player is not defender");
        }

        Players[playerId].IsPassed = true;
        Players[playerId].IsWantToTake = true;

        playerTimers[playerId].Stop();
        await SyncStateAsync();
        await CheckForNextTurnAsync();
    }

    public void ResetPassForAll()
    {
        foreach (var player in Players)
        {
            player.Value.IsPassed = false;
        }
    }

    public async Task CheckForNextTurnAsync()
    {
        if (Players.Where(x => x.Key != CurrentTurn.DefenderId).All(x => x.Value.IsPassed) && IsAllCardsDefendedOrWantToTake())
        {
            if (Players[CurrentTurn.DefenderId].IsWantToTake)
                Take(CurrentTurn.DefenderId);

            await NextTurnAsync();
        }
    }

    public void CheckForGameEnd()
    {
        if (Players.Any(x => x.Value.Cards.Count == 0) && Deck.Count == 0)
        {
            DisposeTimers();
            var winnerId = Players.First(x => x.Value.Cards.Count == 0).Key;

            GameEnded?.Invoke(this, new GameResult(winnerId));
        }
    }

    public GameState GetGameStateForPlayer(long playerId)
    {
        var playerStates = GetPlayerStates();
        var cards = Players[playerId].Cards;
        return new GameState(playerStates, Deck.Count, Trump, TrumpCard, CurrentTurn, cards);
    }

    public List<PlayerState> GetPlayerStates()
    {
        var playerStates = new List<PlayerState>();

        foreach (var player in Players.Values)
        {
            playerStates.Add(new PlayerState(player.Id, player.IsPassed, player.IsWantToTake, player.Cards.Count));
        }

        return playerStates;
    }

    private async Task SendStartGameEventToPlayersAsync()
    {
        foreach (var playerId in PlayerIds)
        {
            var gameState = GetGameStateForPlayer(playerId);
            await _roomEventService.SendAsync(new StartGameEvent(gameState), playerId);
        }
    }

    private async Task SyncStateAsync()
    {
        foreach (var playerId in PlayerIds)
        {
            var gameState = GetGameStateForPlayer(playerId);
            await _roomEventService.SendAsync(new SyncGameStateEvent(gameState), playerId);
        }
    }

    private bool IsAllCardsDefendedOrWantToTake()
    {
        var isWantToTake = Players[CurrentTurn.DefenderId].IsWantToTake;
        return CurrentTurn.TableCards.All(x => x.DefendCard is not null) || isWantToTake;
    }

    private void SetupTimers()
    {
        foreach (var playerId in PlayerIds)
        {
            playerTimers.Add(playerId, new System.Timers.Timer(40000));
            playerTimers[playerId].Elapsed += (sender, e) => TimerTask(playerId);
            playerTimers[playerId].AutoReset = false;
            playerTimers[playerId].Start();
        }
    }

    private void ResetTimers()
    {
        foreach (var playerId in PlayerIds)
        {
            playerTimers[playerId].Stop();
            playerTimers[playerId].Start();
        }
    }

    private void DisposeTimers()
    {
        foreach (var playerId in PlayerIds)
        {
            playerTimers[playerId].Stop();
            playerTimers[playerId].Dispose();
        }
    }

    private void TimerTask(long playerId)
    {
        if (playerId == CurrentTurn.AttackerId)
            SetPassAsync(playerId).Wait();
        else if (playerId == CurrentTurn.DefenderId && CurrentTurn.TableCards.Count != 0)
            SetWantToTakeAsync(playerId).Wait();
        else
            SetPassAsync(playerId).Wait();
    }

    public void Terminate()
    {
        DisposeTimers();
    }

    public void Reset()
    {
        Deck = [];
        TrumpCard = new Card();
        Players = [];
        PlayerIds = [];
        CurrentTurn = new Turn();
        playerTimers.Clear();
    }
}
