namespace Durak.Core.RoomModule.ValueObjects;

public record PrivateRoomSettings(
    int Bet,
    int MaxPlayerCount
) : BetRoomSettings(Bet, MaxPlayerCount);