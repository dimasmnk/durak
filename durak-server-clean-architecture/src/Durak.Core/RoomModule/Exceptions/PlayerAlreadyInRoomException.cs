namespace Durak.Core.RoomModule.Exceptions;

public class PlayerAlreadyInRoomException(Guid roomId, long playerId)
    : Exception($"Player {playerId} has already joined room {roomId}.");