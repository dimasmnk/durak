using Durak.Core.RoomModule.Settings;

namespace Durak.Core.RoomModule.ValueObjects;

public abstract record RoomSettings
{
    protected RoomSettings(int maxPlayerCount)
    {
        if (maxPlayerCount is < RoomConstraints.MinPlayerCount or > RoomConstraints.MaxPlayerCount)
            throw new ArgumentOutOfRangeException(nameof(maxPlayerCount), maxPlayerCount,
                "Wrong maxPlayerCount value.");

        MaxPlayerCount = maxPlayerCount;
    }

    public int MaxPlayerCount { get; }
}