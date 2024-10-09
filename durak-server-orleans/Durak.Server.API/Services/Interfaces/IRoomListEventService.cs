using Durak.Server.API.Hubs;

namespace Durak.Server.API.Services.Interfaces;

public interface IRoomListEventService
{
    Task SendAsync<T>(T @event, CancellationToken cancellationToken) where T : IEvent;
}