using Durak.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Durak.Api.Services;

public class RoomListEventService(IHubContext<RoomListHub> hubContext) : IRoomListEventService
{
    private readonly IHubContext<RoomListHub> _hubContext = hubContext;


    public async Task SendAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : IEvent
    {
        await _hubContext.Clients.All.SendAsync(@event.MethodName, @event, cancellationToken);
    }
}
