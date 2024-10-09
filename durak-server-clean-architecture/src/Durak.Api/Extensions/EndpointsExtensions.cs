using Durak.Api.Endpoints;

namespace Durak.Api.Extensions;

public static class EndpointsExtensions
{
    public static void MapAllEndpoints(this WebApplication app)
    {
        app.MapAuthEndpoints();
    }
}