using Durak.Server.API.Hubs;
using Durak.Server.API.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Durak.Server.API.Services;

public class RoomEventService(IHubContext<RoomHub> hubContext, ISignalrConnectionService signalrConnectionService) : IRoomEventService
{
    private readonly IHubContext<RoomHub> _hubContext = hubContext;
    private readonly ISignalrConnectionService _signalrConnectionService = signalrConnectionService;

    public async Task SendAsync<T>(T @event, string groupKey, CancellationToken cancellationToken = default)
        where T : IEvent
    {
        await _hubContext.Clients.Group(groupKey).SendAsync(@event.MethodName, @event, cancellationToken);
    }

    public async Task SendAsync<T>(T @event, long playerId, CancellationToken cancellationToken = default)
    where T : IEvent
    {
        var connectionId = _signalrConnectionService.GetConnection(playerId);

        if (!string.IsNullOrEmpty(connectionId))
            await _hubContext.Clients.Client(connectionId).SendAsync(@event.MethodName, @event, cancellationToken);
    }
}
