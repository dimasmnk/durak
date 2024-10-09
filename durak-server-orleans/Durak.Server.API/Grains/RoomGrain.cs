using Durak.Server.API.Game.Entities;
using Durak.Server.API.Models;
using Durak.Server.API.Services.Interfaces;

namespace Durak.Server.API.Grains;

public class RoomGrain(IRoomService roomService, IGameService gameService, IPlayerService playerService) : Grain, IRoomGrain
{
    private readonly IRoomService _roomService = roomService;
    private readonly IGameService _gameService = gameService;
    private readonly IPlayerService _playerService = playerService;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await _roomService.InitAsync(this.GetPrimaryKeyString(), cancellationToken);
        _gameService.GameEnded += _roomService.FinishGame!;
        await base.OnActivateAsync(cancellationToken);
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        await _roomService.Terminate(cancellationToken);
        _gameService.Terminate();
        await base.OnDeactivateAsync(reason, cancellationToken);
    }

    public async Task<bool> TryAddPlayerAsync(long playerId, string playerName)
    {
        return await _roomService.TryAddPlayerAsync(playerId, playerName);
    }

    public async Task<bool> TryRemovePlayerAsync(long playerId)
    {
        if (_roomService.IsGameStarted)
        {
            var winnderId = _roomService.Players.Keys.Where(x => x != playerId).FirstOrDefault();
            _gameService.Terminate();
            _roomService.FinishGame(null!, new GameResult(winnderId));
        }

        return await _roomService.TryRemovePlayerAsync(playerId);
    }

    public async Task SetReadyAsync(long playerId)
    {
        _roomService.SetReady(playerId);

        if (_roomService.IsGameStarted)
        {
            var players = _roomService.Players.Keys.ToDictionary(x => x, x => false);

            foreach (var player in _roomService.Players)
            {
                var isWithdrawedSuccessfuly = await _playerService.TryWithdrawAsync(player.Key, (short)_roomService.RoomSettings.Bet);
                players[player.Key] = isWithdrawedSuccessfuly;
            }

            if (players.All(x => x.Value))
            {
                await _gameService.StartGameAsync(_roomService.Players.Keys);
            }
            else
            {
                foreach (var player in players.Where(x => x.Value))
                {
                    await _playerService.DepositAsync(player.Key, (short)_roomService.RoomSettings.Bet);
                }

                throw new Exception("Not all players have enough coins");
            }
        }

        await Task.CompletedTask;
    }

    public async ValueTask<RoomState> GetRoomStateAsync()
    {
        return await Task.FromResult(_roomService.GetRoomState());
    }

    public async ValueTask<GameState> GetGameStateAsync(long playerId)
    {
        if (!_roomService.IsGameStarted)
            throw new InvalidOperationException("Game is not started");

        return await Task.FromResult(_gameService.GetGameStateForPlayer(playerId));
    }

    public async ValueTask AttackAsync(long playerId, Card card)
    {
        await _gameService.AttackAsync(playerId, card);
    }

    public async ValueTask DefendAsync(long playerId, int cardAttackId, Card card)
    {
        await _gameService.DefendAsync(playerId, cardAttackId, card);
    }

    public async ValueTask SetPassAsync(long playerId)
    {
        await _gameService.SetPassAsync(playerId);
    }

    public async ValueTask SetWantToTakeAsync(long playerId)
    {
        await _gameService.SetWantToTakeAsync(playerId);
    }

    public async Task SetOnlineStatusAsync(long playerId, bool isConnected)
    {
        _roomService.Players[playerId].IsConnected = isConnected;
        await Task.CompletedTask;
    }
}
