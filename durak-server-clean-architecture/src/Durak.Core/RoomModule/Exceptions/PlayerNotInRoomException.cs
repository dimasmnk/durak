namespace Durak.Core.RoomModule.Exceptions;

public class PlayerNotInRoomException(Guid roomId, long playerId)
    : Exception($"Player {playerId} is not in room {roomId}.");