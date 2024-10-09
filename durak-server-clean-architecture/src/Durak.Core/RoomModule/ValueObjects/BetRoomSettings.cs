using Durak.Core.RoomModule.Settings;

namespace Durak.Core.RoomModule.ValueObjects;

public abstract record BetRoomSettings : RoomSettings
{
    protected BetRoomSettings(int bet, int maxPlayerCount) : base(maxPlayerCount)
    {
        if (bet is < RoomConstraints.MinBet or > RoomConstraints.MaxBet)
            throw new ArgumentOutOfRangeException(nameof(bet), bet,
                "Wrong bet value.");

        Bet = bet;
    }

    public int Bet { get; }
}