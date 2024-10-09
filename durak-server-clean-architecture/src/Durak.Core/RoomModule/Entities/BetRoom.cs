using Durak.Core.RoomModule.Enums;
using Durak.Core.RoomModule.ValueObjects;
using Durak.Core.UserModule.Entities;

namespace Durak.Core.RoomModule.Entities;

public abstract class BetRoom<TSettings>
    : Room<TSettings>
    where TSettings : BetRoomSettings
{
    protected BetRoom(TSettings settings) : base(settings)
    {
    }

    public override void AddPlayer(User user)
    {
        ValidatePlayerNotInRoom(user.Id);
        ValidateRoomIsNotFull();
        user.Withdraw(Settings.Bet);
        Players.Add(Player.CreatePlayer(user.Id, user.FirstName));
        UpdatePlayerCount();
        UpdateRoomStatus();
    }

    public override void RemovePlayer(User user)
    {
        ValidatePlayerInRoom(user.Id);
        user.Deposit(Settings.Bet);
        var player = Players.Single(x => x.Id == user.Id);
        Players.Remove(player);
        UpdatePlayerCount();
        UpdateRoomStatus();
    }

    public void SetReadyForPlayer(long userId)
    {
        ValidatePlayerInRoom(userId);
        var player = Players.First(x => x.Id == userId);
        player.SetReady();
    }

    public bool AreAllPlayersReady()
    {
        return Players.All(x => x.Status == PlayerStatus.Ready);
    }

    private void UpdateRoomStatus()
    {
        Status = PLayerCount switch
        {
            <= 0 => RoomStatus.Idle,
            >= 1 => RoomStatus.Gathering
        };
    }
}