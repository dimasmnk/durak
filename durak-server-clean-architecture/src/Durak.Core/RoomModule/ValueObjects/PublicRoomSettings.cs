namespace Durak.Core.RoomModule.ValueObjects;

public record PublicRoomSettings(
    int Bet,
    int MaxPlayerCount
) : BetRoomSettings(Bet, MaxPlayerCount);