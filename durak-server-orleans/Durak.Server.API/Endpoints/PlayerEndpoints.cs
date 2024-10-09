using Durak.Server.API.Extensions;
using Durak.Server.API.Services.Interfaces;
using System.Security.Claims;

namespace Durak.Server.API.Endpoints;

public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this WebApplication app)
    {
        app.MapGet("/me", async (ClaimsPrincipal user, IPlayerService playerService, CancellationToken cancellationToken) =>
        {
            var userId = user.GetUserId();
            return await playerService.GetPlayerAsync(userId);
        }).RequireAuthorization();
    }
}
