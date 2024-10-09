using Durak.Server.API.Game.Entities;
using Durak.Server.API.Services;
using Durak.Server.API.Services.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Durak.Server.API.UnitTests.Services;
public class GameServiceTests
{
    private readonly GameService _gameService;
    private readonly IRoomEventService _roomEventService;

    public GameServiceTests()
    {
        _roomEventService = Substitute.For<IRoomEventService>();
        _gameService = new GameService(_roomEventService);
    }

    [Fact]
    public async Task StartGame_TwoPlayers_ValidCardCountMustBeSet()
    {
        // Arrange
        var playerIds = new List<long> { 1, 2 };

        // Act
        await _gameService.StartGameAsync(playerIds);

        // Assert
        Assert.Equal(36 - 6 * 2, _gameService.Deck.Count);
        Assert.Equal(6, _gameService.Players[1].Cards.Count);
        Assert.Equal(6, _gameService.Players[2].Cards.Count);
    }

    [Fact]
    public async Task StartGame_AutoGame_ShouldEndGame()
    {
        // Arrange
        var playerIds = new List<long> { 1, 2 };
        await _gameService.StartGameAsync(playerIds);
        var player1 = _gameService.Players[1].Cards;
        var player2 = _gameService.Players[2].Cards;
        var comparer = new CardComparer(_gameService.Trump);

        // Act
        while (_gameService.Deck.Count > 0 || (player1.Count != 0 && player2.Count != 0))
        {
            var attackerId = _gameService.CurrentTurn.AttackerId;
            var defenderId = _gameService.CurrentTurn.DefenderId;
            var attacker = _gameService.Players[attackerId];
            var defender = _gameService.Players[defenderId];

            var attackerCard = attacker.Cards.First();
            await _gameService.AttackAsync(attackerId, attackerCard);

            var defenderCard = defender.Cards.Where(x => x.Suit == attackerCard.Suit).FirstOrDefault(x => comparer.Compare(x, attackerCard) > 1);

            if (defenderCard is not null)
            {
                await _gameService.DefendAsync(defenderId, 0, defenderCard);
            }
            else
            {
                await _gameService.SetWantToTakeAsync(defenderId);
            }

            await _gameService.SetPassAsync(attackerId);
        }

        // Assert
        Assert.True(player1.Count == 0 || player2.Count == 0);
    }

    [Fact]
    public async Task StartGame_AutoGame_ShouldCallEndGameEvent()
    {
        // Arrange
        var isGameEnded = false;
        _gameService.GameEnded += (sender, args) =>
        {
            isGameEnded = true;
        };
        var playerIds = new List<long> { 1, 2 };
        await _gameService.StartGameAsync(playerIds);
        var player1 = _gameService.Players[1].Cards;
        var player2 = _gameService.Players[2].Cards;
        var comparer = new CardComparer(_gameService.Trump);

        // Act
        while (!isGameEnded)
        {
            var attackerId = _gameService.CurrentTurn.AttackerId;
            var defenderId = _gameService.CurrentTurn.DefenderId;
            var attacker = _gameService.Players[attackerId];
            var defender = _gameService.Players[defenderId];

            var attackerCard = attacker.Cards.First();
            await _gameService.AttackAsync(attackerId, attackerCard);

            var defenderCard = defender.Cards.Where(x => x.Suit == attackerCard.Suit).FirstOrDefault(x => comparer.Compare(x, attackerCard) > 1);

            if (defenderCard is not null)
            {
                await _gameService.DefendAsync(defenderId, 0, defenderCard);
            }
            else
            {
                await _gameService.SetWantToTakeAsync(defenderId);
            }

            await _gameService.SetPassAsync(attackerId);
        }

        // Assert
        Assert.True(player1.Count == 0 || player2.Count == 0);
    }
}
