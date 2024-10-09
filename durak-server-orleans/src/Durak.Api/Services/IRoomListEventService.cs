using Durak.Api.Hubs;

namespace Durak.Api.Services;
public interface IRoomListEventService
{
    Task SendAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent;
}