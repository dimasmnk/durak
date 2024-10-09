using Durak.Server.API.Entities;
using Durak.Server.API.Services.Interfaces;

namespace Durak.Server.API.Grains;

public class PlayerGrain(IPlayerService playerService) : Grain, IPlayerGrain
{
    private readonly IPlayerService _playerService = playerService;

    private Player _player = new();
    private string? _currentRoom;
    private string _name = string.Empty;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _player = await _playerService.GetOrCreatePlayerAsnyc(this.GetPrimaryKeyLong(), cancellationToken);

        await base.OnActivateAsync(cancellationToken);
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        await _playerService.UpdatePlayerAsync(_player, cancellationToken);
        await base.OnDeactivateAsync(reason, cancellationToken);
    }

    public ValueTask<Player> GetPlayerAsync() => ValueTask.FromResult(_player);

    public async Task<bool> TryJoinRoomAsync(string roomId)
    {
        if (_currentRoom is not null) return false;

        var room = GrainFactory.GetGrain<IRoomGrain>(roomId);
        var bet = (short)((await room.GetRoomStateAsync()).RoomSettings.Bet);

        if (_player.CoinCount < bet) return false;

        var isJoined = await room.TryAddPlayerAsync(this.GetPrimaryKeyLong(), _name);

        if (isJoined)
        {
            _currentRoom = roomId;
        }

        return isJoined;
    }

    public async Task<bool> TryLeaveRoomAsync()
    {
        if (_currentRoom is null) return false;

        var room = GrainFactory.GetGrain<IRoomGrain>(_currentRoom);

        var isRemoved = await room.TryRemovePlayerAsync(this.GetPrimaryKeyLong());

        if (isRemoved)
        {
            _currentRoom = null;
        }

        return isRemoved;
    }

    public async Task<string> GetCurrentRoomIdAsync()
    {
        if (_currentRoom is null) return string.Empty;

        return await Task.FromResult(_currentRoom);
    }

    public async Task SetNameAsync(string name)
    {
        _name = name;
        await Task.CompletedTask;
    }

    public async ValueTask<string> GetNameAsync()
    {
        return await Task.FromResult(_name);
    }

    public async ValueTask<bool> TryWithdrawAsync(long coinCount)
    {
        if (_player.CoinCount < coinCount) return false;

        _player.CoinCount -= coinCount;

        return await Task.FromResult(true);
    }

    public async ValueTask DepositAsync(long coinCount)
    {
        _player.CoinCount += coinCount;
        await Task.CompletedTask;
    }

    public async ValueTask UpdatePlayerStatisticAsync(bool isWin)
    {
        if (isWin)
        {
            _player.Wins++;
        }
        else
        {
            _player.Losses++;
        }

        _player.Total++;

        await Task.CompletedTask;
    }
}
