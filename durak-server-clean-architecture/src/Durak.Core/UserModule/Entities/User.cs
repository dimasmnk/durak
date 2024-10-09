using Durak.Core.UserModule.Exceptions;
using Durak.Core.UserModule.Settings;

namespace Durak.Core.UserModule.Entities;

public class User
{
    private User()
    {
    }

    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public int CoinCount { get; private set; }
    public int WinCount { get; }
    public int LossCount { get; }
    public int DrawCount { get; }
    public int TotalCount { get; }

    public static User CreateUser(string firstName)
    {
        if (firstName.Length > UserSettings.FirstNameMaxLength)
            throw new ArgumentOutOfRangeException(nameof(firstName), firstName, "First name is too long.");

        return new User
        {
            FirstName = firstName,
            CoinCount = UserSettings.StartCoinCount
        };
    }

    public void Withdraw(int coinCountToWithdraw)
    {
        if (coinCountToWithdraw < 0)
            throw new ArgumentOutOfRangeException(nameof(coinCountToWithdraw), coinCountToWithdraw,
                "coinCountToWithdraw cannot be less than zero.");

        if (CoinCount < coinCountToWithdraw)
            throw new InsufficientBalanceException(Id, CoinCount, coinCountToWithdraw);

        CoinCount -= coinCountToWithdraw;
    }

    public void Deposit(int coinContToDeposit)
    {
        if (coinContToDeposit < 0)
            throw new ArgumentOutOfRangeException(nameof(coinContToDeposit), coinContToDeposit,
                "coinCountToWithdraw cannot be less than zero.");

        CoinCount += coinContToDeposit;
    }
}