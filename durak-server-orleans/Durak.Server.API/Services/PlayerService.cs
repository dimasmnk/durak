using Durak.Server.API.Context;
using Durak.Server.API.Entities;
using Durak.Server.API.Game.Entities;
using Durak.Server.API.Grains;
using Durak.Server.API.Repositories;
using Durak.Server.API.Services.Interfaces;

namespace Durak.Server.API.Services;

public class PlayerService(IPlayerRepository playerRepository, IDurakDbContext durakDbContext, IGrainFactory grainFactory) : IPlayerService
{
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IDurakDbContext _durakDbContext = durakDbContext;
    private readonly IGrainFactory _grainFactory = grainFactory;

    public async Task<Player> GetOrCreatePlayerAsnyc(long id, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetPlayerByIdAsync(id, cancellationToken);

        if (player is null)
        {
            player = (await _playerRepository.CreatePlayerAsync(CreateDefaultPlayer(id), cancellationToken)).Entity;
            await _durakDbContext.SaveChangesAsync(cancellationToken);
        }

        return player;
    }

    public async Task UpdatePlayerAsync(Player player, CancellationToken cancellationToken)
    {
        _playerRepository.UpdatePlayer(player);
        await _durakDbContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask<Player> GetPlayerAsync(long playerId)
    {
        var playerGrain = _grainFactory.GetGrain<IPlayerGrain>(playerId);
        return await playerGrain.GetPlayerAsync();
    }

    public async Task SetPlayerNameAsync(long playerId, string name)
    {
        var playerGrain = _grainFactory.GetGrain<IPlayerGrain>(playerId);
        await playerGrain.SetNameAsync(name);
    }

    public async ValueTask<string> GetCurrentRoomId(long playerId)
    {
        var playerGrain = _grainFactory.GetGrain<IPlayerGrain>(playerId);
        return await playerGrain.GetCurrentRoomIdAsync();
    }

    public async Task<bool> TryJoinRoomAsync(long playerId, string roomId, CancellationToken cancellationToken)
    {
        var playerGrain = _grainFactory.GetGrain<IPlayerGrain>(playerId);
        return await playerGrain.TryJoinRoomAsync(roomId);
    }

    public async Task<bool> TryLeaveRoomAsync(long playerId)
    {
        var playerGrain = _grainFactory.GetGrain<IPlayerGrain>(playerId);
        return await playerGrain.TryLeaveRoomAsync();
    }

    public async Task SetReadyAsync(long playerId)
    {
        var playerGrain = _grainFactory.GetGrain<IPlayerGrain>(playerId);
        var currentRoomId = await playerGrain.GetCurrentRoomIdAsync();
        var roomGrain = _grainFactory.GetGrain<IRoomGrain>(currentRoomId);
        await roomGrain.SetReadyAsync(playerId);
    }

    public async ValueTask AttackAsync(long playerId, Card card)
    {
        var playerGrain = _grainFactory.GetGrain<IPlayerGrain>(playerId);
        var currentRoomId = await playerGrain.GetCurrentRoomIdAsync();
        var roomGrain = _grainFactory.GetGrain<IRoomGrain>(currentRoomId);
        await roomGrain.AttackAsync(playerId, card);
    }

    public async ValueTask DefendAsync(long playerId, int cardAttackId, Card card)
    {
        var playerGrain = _grainFactory.GetGrain<IPlayerGrain>(playerId);
        var currentRoomId = await playerGrain.GetCurrentRoomIdAsync();
        var roomGrain = _grainFactory.GetGrain<IRoomGrain>(currentRoomId);
        await roomGrain.DefendAsync(playerId, cardAttackId, card);
    }

    public async ValueTask SetPassAsync(long playerId)
    {
        var playerGrain = _grainFactory.GetGrain<IPlayerGrain>(playerId);
        var currentRoomId = await playerGrain.GetCurrentRoomIdAsync();
        var roomGrain = _grainFactory.GetGrain<IRoomGrain>(currentRoomId);
        await roomGrain.SetPassAsync(playerId);
    }

    public async ValueTask SetWantToTakeAsync(long playerId)
    {
        var playerGrain = _grainFactory.GetGrain<IPlayerGrain>(playerId);
        var currentRoomId = await playerGrain.GetCurrentRoomIdAsync();
        var roomGrain = _grainFactory.GetGrain<IRoomGrain>(currentRoomId);
        await roomGrain.SetWantToTakeAsync(playerId);
    }

    public async ValueTask<bool> TryWithdrawAsync(long playerId, long coinCount)
    {
        var playerGrain = _grainFactory.GetGrain<IPlayerGrain>(playerId);
        return await playerGrain.TryWithdrawAsync(coinCount);
    }

    public async Task DepositAsync(long playerId, long coinCount)
    {
        var playerGrain = _grainFactory.GetGrain<IPlayerGrain>(playerId);
        await playerGrain.DepositAsync(coinCount);
    }

    public async Task UpdatePlayerStatisticAsync(long playerId, bool isWin)
    {
        var playerGrain = _grainFactory.GetGrain<IPlayerGrain>(playerId);
        await playerGrain.UpdatePlayerStatisticAsync(isWin);
    }

    private static Player CreateDefaultPlayer(long id) => new()
    {
        Id = id,
        CoinCount = 100,
        Wins = 0,
        Losses = 0,
        Total = 0,
    };
}
