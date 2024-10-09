namespace Durak.Core.GameModule.Exceptions;

public class PlayerIsNotDefenderException(long playerId)
    : Exception($"Player {playerId} is not defender.");