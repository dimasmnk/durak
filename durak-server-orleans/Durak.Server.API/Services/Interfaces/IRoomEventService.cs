using Durak.Server.API.Hubs;

namespace Durak.Server.API.Services.Interfaces;

public interface IRoomEventService
{
    Task SendAsync<T>(T @event, string groupKey, CancellationToken cancellationToken = default) where T : IEvent;
    Task SendAsync<T>(T @event, long playerId, CancellationToken cancellationToken = default) where T : IEvent;
}