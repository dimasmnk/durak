using Durak.Api.Endpoints;

namespace Durak.Api.Extensions;

public static class EndpointExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapAuthenticationEndpoints();
        app.MapUserEndpoints();
        app.MapRoomEndpoints();
    }
}
