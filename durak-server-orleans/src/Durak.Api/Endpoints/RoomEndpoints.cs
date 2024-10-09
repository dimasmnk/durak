using Durak.Api.Contracts.Requests;
using Durak.Api.Extensions;
using Durak.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Durak.Api.Endpoints;

public static class RoomEndpoints
{
    public static void MapRoomEndpoints(this WebApplication app)
    {
        var roomEndpoints = app.MapGroup("/rooms").RequireAuthorization();

        roomEndpoints.MapPost("/", async ([FromBody] CreateRoomRequest createRoomRequest, ClaimsPrincipal claimUser, IRoomService roomService, CancellationToken cancellationToken) =>
        {
            var userId = claimUser.GetUserId();
            var roomId = await roomService.CreateRoomAsync(createRoomRequest, userId, cancellationToken);
            return Results.Ok(roomId);
        });

        roomEndpoints.MapPost("/{roomId:guid}", async (Guid roomId, ClaimsPrincipal claimUser, IRoomService roomService, CancellationToken cancellationToken) =>
        {
            var userId = claimUser.GetUserId();
            var isJoined = await roomService.TryJoinRoomAsync(userId, roomId, cancellationToken);
            return isJoined ? Results.Ok() : Results.Problem();
        });

        roomEndpoints.MapGet("/", async (IRoomService roomService, CancellationToken cancellationToken) =>
        {
            var rooms = await roomService.GetPublicRoomsAsync(cancellationToken);
            return Results.Ok(rooms);
        });
    }
}
