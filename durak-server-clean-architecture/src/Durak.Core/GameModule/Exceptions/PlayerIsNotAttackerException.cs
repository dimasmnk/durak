namespace Durak.Core.GameModule.Exceptions;

public class PlayerIsNotAttackerException(long playerId)
    : Exception($"Player {playerId} is not attacker.");