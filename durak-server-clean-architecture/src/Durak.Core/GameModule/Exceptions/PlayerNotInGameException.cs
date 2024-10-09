namespace Durak.Core.GameModule.Exceptions;

public class PlayerNotInGameException(Guid gameId, long playerId)
    : Exception($"Player {playerId} is not in game {gameId}.");