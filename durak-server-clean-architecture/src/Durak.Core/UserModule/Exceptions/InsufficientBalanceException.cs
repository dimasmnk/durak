namespace Durak.Core.UserModule.Exceptions;

public class InsufficientBalanceException(long playerId, int currentBalance, int withdrawAmount)
    : Exception($"Insufficient balance of player {playerId}. Current: {currentBalance} Withdraw: {withdrawAmount}");