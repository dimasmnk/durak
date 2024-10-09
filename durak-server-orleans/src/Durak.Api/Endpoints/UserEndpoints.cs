using Durak.Api.Extensions;
using Durak.Api.Services;
using System.Security.Claims;

namespace Durak.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var userEndpoints = app.MapGroup("/users").RequireAuthorization();

        userEndpoints.MapGet("/me", async (ClaimsPrincipal claimUser, IUserService userService, CancellationToken cancellationToken) =>
        {
            var userId = claimUser.GetUserId();
            var user = await userService.GetOrCreateUserAsync(userId, cancellationToken);
            return Results.Ok(user);
        });

        userEndpoints.MapPost("/sync", async (ClaimsPrincipal claimUser, IUserService userService, CancellationToken cancellationToken) =>
        {
            var userId = claimUser.GetUserId();
            var username = claimUser.GetUsername();
            await userService.UpdateUsernameAsync(userId, username, cancellationToken);
            return Results.Ok();
        });
    }
}
