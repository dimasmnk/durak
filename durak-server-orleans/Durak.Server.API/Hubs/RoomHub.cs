using Durak.Server.API.Extensions;
using Durak.Server.API.Grains;
using Durak.Server.API.Hubs.RoomEvents;
using Durak.Server.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Durak.Server.API.Hubs;

[Authorize]
public class RoomHub(IGrainFactory grainFactory, ISignalrConnectionService signalrConnectionService, IRoomEventService roomEventService) : Hub
{
    private readonly IGrainFactory _grainFactory = grainFactory;
    private readonly ISignalrConnectionService _signalrConnectionService = signalrConnectionService;
    private readonly IRoomEventService _roomEventService = roomEventService;

    public override async Task OnConnectedAsync()
    {
        var playerId = Context!.User!.GetUserId();
        var player = _grainFactory.GetGrain<IPlayerGrain>(playerId);
        var roomId = await player.GetCurrentRoomIdAsync();
        if (string.IsNullOrEmpty(roomId)) return;
        await Groups.AddToGroupAsync(Context?.ConnectionId!, roomId);
        _signalrConnectionService.AddConnection(playerId, Context!.ConnectionId!);
        var room = _grainFactory.GetGrain<IRoomGrain>(roomId);
        await room.SetOnlineStatusAsync(playerId, true);
        await _roomEventService.SendAsync(new PlayerConnectionStatusEvent(playerId, true), roomId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var playerId = Context!.User!.GetUserId();
        var player = _grainFactory.GetGrain<IPlayerGrain>(playerId);
        var roomId = await player.GetCurrentRoomIdAsync();
        if (!string.IsNullOrEmpty(roomId))
        {
            var room = _grainFactory.GetGrain<IRoomGrain>(roomId);
            if (room != null)
            {
                var roomState = await room.GetRoomStateAsync();
                if (!roomState.IsGameStarted)
                {
                    await player.TryLeaveRoomAsync();
                }
                else
                {
                    await room.SetOnlineStatusAsync(playerId, false);
                    await _roomEventService.SendAsync(new PlayerConnectionStatusEvent(playerId, false), roomId);
                }
            }
        }

        _signalrConnectionService.RemoveConnection(playerId);
        await base.OnDisconnectedAsync(exception);
    }
}
