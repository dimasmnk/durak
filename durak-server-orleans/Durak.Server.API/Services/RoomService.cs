using Durak.Server.API.Game.Entities;
using Durak.Server.API.Hubs.RoomEvents;
using Durak.Server.API.Models;
using Durak.Server.API.Services.Interfaces;

namespace Durak.Server.API.Services;

public class RoomService(IRoomListService roomListService, IRoomTokenService roomTokenService, IRoomEventService roomEventService, IPlayerService playerService) : IRoomService
{
    private readonly IRoomListService _roomListService = roomListService;
    private readonly IRoomTokenService _roomTokenService = roomTokenService;
    private readonly IRoomEventService _roomEventService = roomEventService;
    private readonly IPlayerService _playerService = playerService;

    public string Key { get; private set; } = string.Empty;
    public Dictionary<long, RoomPlayer> Players { get; private set; } = [];
    public RoomSettings RoomSettings { get; private set; } = new(Guid.Empty, Enums.Bet.None, false);
    public bool IsPublic => !RoomSettings.IsPrivate;
    public int MaxPlayerCount { get; } = 2;
    public bool AreAllReady => Players.All(x => x.Value.IsReady) && Players.Count >= MaxPlayerCount;
    public bool IsGameStarted { get; set; } = false;

    public async Task InitAsync(string key, CancellationToken cancellationToken)
    {
        Key = key;

        RoomSettings = _roomTokenService.ConvertTokenToRoomSettings(key);

        if (IsPublic)
            await _roomListService.AddRoomAsync(Key, RoomSettings, cancellationToken);
    }

    public async Task Terminate(CancellationToken cancellationToken)
    {
        if (IsPublic)
            await _roomListService.RemoveRoomAsync(Key, cancellationToken);
    }

    public async Task<bool> TryAddPlayerAsync(long playerId, string playerName)
    {
        if (Players.Count >= MaxPlayerCount) return false;

        var newPlayer = new RoomPlayer(playerId, playerName, false, true);

        Players.Add(playerId, newPlayer);

        if (IsPublic)
            await _roomListService.UpdateRoomAsync(Key, (byte)Players.Count, default);

        await _roomEventService.SendAsync(new JoinPlayerEvent(newPlayer), Key);

        return true;
    }

    public async Task<bool> TryRemovePlayerAsync(long playerId)
    {
        if (Players.Count == 0) return false;

        Players.Remove(playerId);

        if (IsPublic)
            await _roomListService.UpdateRoomAsync(Key, Players.Count, default);

        await _roomEventService.SendAsync(new LeavePlayerEvent(playerId), Key);

        return true;
    }

    public async void SetReady(long playerId)
    {
        if (Players.TryGetValue(playerId, out var player))
            player.IsReady = true;

        if (AreAllReady)
        {
            IsGameStarted = true;
        }

        await _roomEventService.SendAsync(new SetReadyEvent(playerId), Key);
    }

    public RoomState GetRoomState()
    {
        return new RoomState([.. Players.Values], IsGameStarted, RoomSettings);
    }

    public void Reset()
    {
        foreach (var player in Players.Values)
        {
            player.IsReady = false;
        }

        IsGameStarted = false;
    }

    public void FinishGame(object sender, GameResult gameResult)
    {
        _playerService.DepositAsync(gameResult.WinnerId, (short)RoomSettings.Bet * 2);
        foreach (var player in Players)
        {
            _playerService.UpdatePlayerStatisticAsync(player.Key, player.Key == gameResult.WinnerId);
        }

        Reset();

        _roomEventService.SendAsync(new GameEndEvent(gameResult.WinnerId), Key);
    }
}
