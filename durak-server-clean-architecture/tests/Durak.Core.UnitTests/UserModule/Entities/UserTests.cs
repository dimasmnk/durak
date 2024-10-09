using Durak.Core.UserModule.Entities;
using Durak.Core.UserModule.Exceptions;
using Durak.Core.UserModule.Settings;

namespace Durak.Core.UnitTests.UserModule.Entities;

public class UserTests
{
    [Fact]
    public void CreateUser_ShouldInitializeWithDefaultValues_WhenValidFirstNameIsProvided()
    {
        // Arrange
        var firstName = "John Doe";

        // Act
        var user = User.CreateUser(firstName);

        // Assert
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(UserSettings.StartCoinCount, user.CoinCount);
        Assert.Equal(0, user.WinCount);
        Assert.Equal(0, user.LossCount);
        Assert.Equal(0, user.DrawCount);
        Assert.Equal(0, user.TotalCount);
    }

    [Fact]
    public void CreateUser_ShouldThrowException_WhenFirstNameIsTooLong()
    {
        // Arrange
        var longFirstName = new string('a', UserSettings.FirstNameMaxLength + 1);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => User.CreateUser(longFirstName));
    }

    [Fact]
    public void Withdraw_ShouldDecreaseCoinCount_WhenValidAmountIsWithdrawn()
    {
        // Arrange
        var initialCoins = UserSettings.StartCoinCount;
        var user = User.CreateUser("John Doe");

        // Act
        user.Withdraw(10);

        // Assert
        Assert.Equal(initialCoins - 10, user.CoinCount);
    }

    [Fact]
    public void Withdraw_ShouldThrowException_WhenInsufficientBalance()
    {
        // Arrange
        var user = User.CreateUser("John Doe");

        // Act & Assert
        Assert.Throws<InsufficientBalanceException>(() => user.Withdraw(UserSettings.StartCoinCount + 1));
    }

    [Fact]
    public void Withdraw_ShouldThrowException_WhenNegativeAmountIsWithdrawn()
    {
        // Arrange
        var user = User.CreateUser("John Doe");

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => user.Withdraw(-10));
    }

    [Fact]
    public void Deposit_ShouldIncreaseCoinCount_WhenValidAmountIsDeposited()
    {
        // Arrange
        var user = User.CreateUser("John Doe");

        // Act
        user.Deposit(20);

        // Assert
        Assert.Equal(UserSettings.StartCoinCount + 20, user.CoinCount);
    }

    [Fact]
    public void Deposit_ShouldThrowException_WhenNegativeAmountIsDeposited()
    {
        // Arrange
        var user = User.CreateUser("John Doe");

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => user.Deposit(-10));
    }
}