using Durak.Api.Hubs;

namespace Durak.Api.Extensions;

public static class SignalrEndpointExtensions
{
    public static void MapSignalrEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<RoomListHub>("/hubs/roomList");
    }
}
