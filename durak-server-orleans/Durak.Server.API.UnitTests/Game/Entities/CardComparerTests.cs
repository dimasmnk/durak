using Durak.Server.API.Game.Entities;
using Durak.Server.API.Game.Enums;
using FluentAssertions;

namespace Durak.Server.API.UnitTests.Game.Entities;

public class CardComparerTests
{
    [Theory]
    [InlineData(Suit.Clubs, Rank.Ace, Suit.Clubs, Rank.Ace, 0)]
    [InlineData(Suit.Clubs, Rank.Ace, Suit.Clubs, Rank.King, 1)]
    [InlineData(Suit.Clubs, Rank.Six, Suit.Diamonds, Rank.Ace, 1)]
    [InlineData(Suit.Clubs, Rank.Ace, Suit.Diamonds, Rank.King, 1)]
    [InlineData(Suit.Clubs, Rank.Six, Suit.Hearts, Rank.Ace, 1)]
    [InlineData(Suit.Hearts, Rank.Ace, Suit.Clubs, Rank.Six, -1)]
    [InlineData(Suit.Hearts, Rank.Six, Suit.Hearts, Rank.Ace, -8)]
    public void Copmare_Cards_ShouldReturnValidResult(Suit suitX, Rank rankX, Suit suitY, Rank rankY, int expectedResult)
    {
        // Arrange
        var cardX = new Card(rankX, suitX);
        var cardY = new Card(rankY, suitY);
        var comparer = new CardComparer(Suit.Clubs);

        // Act
        var result = comparer.Compare(cardX, cardY);

        // Assert
        result.Should().Be(expectedResult);
    }
}
