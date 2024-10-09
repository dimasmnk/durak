using Durak.Server.API.Game.Entities;
using Durak.Server.API.Game.Enums;
using Durak.Server.API.Models;

namespace Durak.Server.API.Services.Interfaces;
public interface IGameService
{
    Turn CurrentTurn { get; set; }
    Stack<Card> Deck { get; set; }
    List<long> PlayerIds { get; set; }
    Dictionary<long, GamePlayer> Players { get; set; }
    Suit Trump { get; }
    Card TrumpCard { get; set; }
    event EventHandler<GameResult>? GameEnded;

    Task AttackAsync(long playerId, Card card);
    void CheckForGameEnd();
    Task CheckForNextTurnAsync();
    void DealCard(long playerId);
    void DealCards();
    Task DefendAsync(long playerId, int cardAttackId, Card defendCard);
    GameState GetGameStateForPlayer(long playerId);
    List<PlayerState> GetPlayerStates();
    Task NextTurnAsync();
    void ResetPassForAll();
    void SetAttackerAndDefender();
    Task SetPassAsync(long playerId);
    void SetupPlayers(IEnumerable<long> playerIds);
    Task SetWantToTakeAsync(long playerId);
    Task StartGameAsync(IEnumerable<long> playerIds);
    void Take(long playerId);
    void Terminate();
}