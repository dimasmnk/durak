using Durak.Core.GameModule.ValueObjects;

namespace Durak.Core.GameModule.Exceptions;

public class CardNotFoundForPlayerException(long playerId, Card card)
    : Exception($"Player {playerId} does not have a card {card}.");