namespace Durak.Core.RoomModule.Exceptions;

public class RoomIfFullException(Guid id) : Exception($"Room {id} is full.");