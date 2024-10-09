using Durak.Core.RoomModule.Enums;
using Durak.Core.RoomModule.Exceptions;
using Durak.Core.RoomModule.ValueObjects;
using Durak.Core.UserModule.Entities;

namespace Durak.Core.RoomModule.Entities;

public abstract class Room<TSettings>
    where TSettings : RoomSettings
{
    protected Room(TSettings settings)
    {
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        Id = Guid.NewGuid();
        Status = RoomStatus.Idle;
        Players = [];
    }

    public Guid Id { get; init; }
    public abstract RoomType Type { get; }

    public RoomStatus Status { get; protected set; }
    public TSettings Settings { get; protected set; }
    public int PLayerCount { get; protected set; }
    public List<Player> Players { get; protected set; }

    public abstract void AddPlayer(User user);
    public abstract void RemovePlayer(User user);

    private bool IsPlayerInRoom(long id)
    {
        return Players.Any(x => x.Id == id);
    }

    protected void UpdatePlayerCount()
    {
        PLayerCount = Players.Count;
    }

    protected void ValidatePlayerNotInRoom(long userId)
    {
        if (IsPlayerInRoom(userId)) throw new PlayerAlreadyInRoomException(Id, userId);
    }

    protected void ValidatePlayerInRoom(long userId)
    {
        if (!IsPlayerInRoom(userId)) throw new PlayerNotInRoomException(Id, userId);
    }

    protected void ValidateRoomIsNotFull()
    {
        if (Players.Count >= Settings.MaxPlayerCount)
            throw new RoomIfFullException(Id);
    }
}