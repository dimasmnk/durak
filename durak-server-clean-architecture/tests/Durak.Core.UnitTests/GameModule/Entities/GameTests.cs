using Durak.Core.GameModule.Constants;
using Durak.Core.GameModule.Entities;
using Durak.Core.GameModule.Enums;
using Durak.Core.GameModule.Exceptions;
using Durak.Core.GameModule.Helpers;
using Durak.Core.GameModule.ValueObjects;

namespace Durak.Core.UnitTests.GameModule.Entities;

public class GameTests
{
    [Fact]
    public void CreateGame_NewGame_ShouldCreateTurnForGameStart()
    {
        // Arrange
        var players = new Dictionary<int, long>
        {
            { 0, 10 },
            { 1, 11 },
            { 2, 12 },
            { 3, 13 }
        };

        // Act
        var game = Game.CreateGame(Guid.NewGuid(), players);

        // Assert
        Assert.NotNull(game);
        Assert.Equal(players.Count, game.Players.Count);
        Assert.Equal(1, game.Tick);
        Assert.True(game.Players.All(x => x.Hand.Count == GameConstants.HandCardCount));
    }

    [Fact]
    public void Attack_ValidCard_ShouldAddToTurn()
    {
        // Arrange
        var game = CreateBasicGame();
        var attackPlayer = game.Players.First(x => x.Id == game.Turn.AttackerId);
        var attackCard = attackPlayer.Hand.First();

        // Act
        game.Attack(attackPlayer.Id, attackCard);

        // Assert
        Assert.Equal(attackCard, game.Turn.CardPairs.First().Value.AttackCard);
        Assert.Equal(2, game.Tick);
    }

    [Fact]
    public void Attack_NotAttacker_ShouldThrowException()
    {
        // Arrange
        var game = CreateBasicGame();
        var attackPlayer = game.Players.First(x => x.Id != game.Turn.AttackerId);
        var attackCard = attackPlayer.Hand.First();

        // Act
        var ex = Record.Exception(() => game.Attack(attackPlayer.Id, attackCard));

        // Assert
        Assert.NotNull(ex);
        Assert.IsType<PlayerIsNotAttackerException>(ex);
    }

    [Fact]
    public void Defend_OneAttackCard_ShouldBeat()
    {
        // Arrange
        var game = CreateBasicGame();
        var attackPlayer = game.Players.First(x => x.Id == game.Turn.AttackerId);
        var defensePlayer = game.Players.First(x => x.Id == game.Turn.DefenderId);
        var attackCard = new Card(Suit.Hearts, Rank.Six);
        var defenseCard = new Card(Suit.Hearts, Rank.Ace);
        attackPlayer.Hand.Add(attackCard);
        defensePlayer.Hand.Add(defenseCard);
        game.Attack(game.Turn.AttackerId, attackCard);

        // Act
        game.Defend(defensePlayer.Id, game.Turn.CardPairs.First().Key, defenseCard);

        // Assert
        Assert.Equal(attackCard, game.Turn.CardPairs.First().Value.AttackCard);
        Assert.Equal(defenseCard, game.Turn.CardPairs.First().Value.DefenseCard);
    }

    [Fact]
    public void Defend_HaveOnePair_AnotherPlayerCanThrowIn()
    {
        // Arrange
        var game = CreateBasicGame();
        var attackPlayer = game.Players.First(x => x.Id == game.Turn.AttackerId);
        var defensePlayer = game.Players.First(x => x.Id == game.Turn.DefenderId);
        var throwInPlayer = game.Players.First(x =>
            x.Id != game.Turn.AttackerId
            && x.Id != game.Turn.DefenderId);
        var attackCard = new Card(Suit.Hearts, Rank.Six);
        var defenseCard = new Card(Suit.Hearts, Rank.Ace);
        var throwInCard = new Card(Suit.Clubs, Rank.Six);
        attackPlayer.Hand.Add(attackCard);
        defensePlayer.Hand.Add(defenseCard);
        throwInPlayer.Hand.Add(throwInCard);
        game.Attack(game.Turn.AttackerId, attackCard);
        game.Defend(game.Turn.DefenderId, game.Turn.CardPairs.First().Key, defenseCard);

        // Act
        game.Attack(throwInPlayer.Id, throwInCard);

        // Assert
        Assert.Equal(attackCard, game.Turn.CardPairs.First().Value.AttackCard);
        Assert.Equal(defenseCard, game.Turn.CardPairs.First().Value.DefenseCard);
        Assert.Equal(throwInCard, game.Turn.CardPairs.Skip(1).First().Value.AttackCard);
    }

    [Fact]
    public void Pass_PlayerCanPass_ShouldSetPass()
    {
        // Arrange
        var game = CreateBasicGame();
        var attackPlayer = game.Players.First(x => x.Id == game.Turn.AttackerId);
        var defensePlayer = game.Players.First(x => x.Id == game.Turn.DefenderId);
        var throwInPlayer = game.Players.First(x =>
            x.Id != game.Turn.AttackerId
            && x.Id != game.Turn.DefenderId);
        var attackCard = new Card(Suit.Hearts, Rank.Six);
        var defenseCard = new Card(Suit.Hearts, Rank.Ace);
        var throwInCard = new Card(Suit.Clubs, Rank.Six);
        attackPlayer.Hand.Add(attackCard);
        defensePlayer.Hand.Add(defenseCard);
        throwInPlayer.Hand.Add(throwInCard);
        game.Attack(game.Turn.AttackerId, attackCard);
        game.Defend(game.Turn.DefenderId, game.Turn.CardPairs.First().Key, defenseCard);

        // Act
        game.Pass(attackPlayer.Id);

        // Assert
        Assert.Equal(PlayerStatus.Pass, game.Players
            .First(x => x.Id == attackPlayer.Id).Status);
    }

    [Fact]
    public void Pass_PlayerCanTake_ShouldSetTake()
    {
        // Arrange
        var game = CreateBasicGame();
        var attackPlayer = game.Players.First(x => x.Id == game.Turn.AttackerId);
        var defensePlayer = game.Players.First(x => x.Id == game.Turn.DefenderId);
        var throwInPlayer = game.Players.First(x =>
            x.Id != game.Turn.AttackerId
            && x.Id != game.Turn.DefenderId);
        var attackCard = new Card(Suit.Hearts, Rank.Six);
        var defenseCard = new Card(Suit.Hearts, Rank.Ace);
        var throwInCard = new Card(Suit.Clubs, Rank.Six);
        attackPlayer.Hand.Add(attackCard);
        defensePlayer.Hand.Add(defenseCard);
        throwInPlayer.Hand.Add(throwInCard);
        game.Attack(game.Turn.AttackerId, attackCard);

        // Act
        game.Take(defensePlayer.Id);

        // Assert
        Assert.Equal(PlayerStatus.Take, game.Players
            .First(x => x.Id == defensePlayer.Id).Status);
    }

    [Fact]
    public void Pass_AllCardsBeaten_ShouldSetNextTurn()
    {
        // Arrange
        var game = CreateBasicGame();
        var attackPlayer = game.Players.First(x => x.Id == game.Turn.AttackerId);
        var defensePlayer = game.Players.First(x => x.Id == game.Turn.DefenderId);
        var throwInPlayer = game.Players.First(x =>
            x.Id != game.Turn.AttackerId
            && x.Id != game.Turn.DefenderId);
        var attackCard = new Card(Suit.Hearts, Rank.Six);
        var defenseCard = new Card(Suit.Hearts, Rank.Ace);
        var throwInCard = new Card(Suit.Clubs, Rank.Six);
        attackPlayer.Hand.Add(attackCard);
        defensePlayer.Hand.Add(defenseCard);
        throwInPlayer.Hand.Add(throwInCard);
        game.Attack(game.Turn.AttackerId, attackCard);
        game.Defend(game.Turn.DefenderId, game.Turn.CardPairs.First().Key, defenseCard);

        // Act
        var players = game.Players.Where(x => x.Id != game.Turn.DefenderId);
        foreach (var player in players) game.Pass(player.Id);

        // Assert
        Assert.Equal(4, game.Tick);
        Assert.Empty(game.Turn.CardPairs);
    }

    [Fact]
    public void IsGameFinished_GameIsFinite_ShouldReturnTrue()
    {
        // Arrange
        var random = new Random();
        var game = CreateBasicGame();
        var comparer = new CardComparer(game.Trump);

        // Act
        while (game.IsGameFinished() == false)
        {
            var attacker = game.Attacker;
            var defender = game.Defender;
            var attackCard = attacker.Hand.OrderBy(_ => random.Next()).First();
            game.Attack(attacker.Id, attackCard);
            var defendCard = defender.Hand
                .Where(x => x.Suit == attackCard.Suit || x.Suit == game.Trump)
                .FirstOrDefault(x =>
                    comparer.Compare(x, game.Turn.CardPairs.First().Value.AttackCard) > 0);

            if (defendCard != null)
            {
                game.Defend(defender.Id, game.Turn.CardPairs.First().Key, defendCard);
                if (game.Attacker != null)
                    game.Pass(attacker.Id);
            }
            else
            {
                game.Take(defender.Id);
            }

            foreach (var player in game.Players.Where(x => x.Id != attacker.Id && x.Id != defender.Id).ToList())
                game.Pass(player.Id);
        }

        var gameResult = game.GetGameResult();

        // Assert
        Assert.True(game.IsGameFinished());
    }

    private static Game CreateBasicGame()
    {
        var players = new Dictionary<int, long>
        {
            { 0, 10 },
            { 1, 11 },
            { 2, 12 },
            { 3, 13 }
        };

        return Game.CreateGame(Guid.NewGuid(), players);
    }
}