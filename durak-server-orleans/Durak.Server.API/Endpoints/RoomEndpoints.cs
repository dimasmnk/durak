using Durak.Server.API.Endpoints.Requests;
using Durak.Server.API.Enums;
using Durak.Server.API.Extensions;
using Durak.Server.API.Game.Entities;
using Durak.Server.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Durak.Server.API.Endpoints;

public static class RoomEndpoints
{
    public static void MapRoomEndpoints(this WebApplication app)
    {
        var roomEndpoints = app.MapGroup("/rooms").RequireAuthorization();

        roomEndpoints.MapGet("/create", (Bet bet, bool isPrivate, IRoomTokenService roomService, CancellationToken cancellationToken) =>
        {
            var roomSettings = roomService.CreateRoomSettings(bet, isPrivate);
            return roomService.ConvertRoomSettingsToToken(roomSettings);
        });

        roomEndpoints.MapGet("", (IRoomListService roomListService, CancellationToken cancellationToken) =>
        {
            return roomListService.Rooms.Where(x => x.PlayerCount > 0);
        });

        roomEndpoints.MapGet("/random", (IRoomListService roomListService) =>
        {
            return roomListService.GetRandomAvailableRoomConnectionId();
        });

        roomEndpoints.MapPost("/join/{roomId}", async (ClaimsPrincipal user, string roomId, IPlayerService playerService, CancellationToken cancellationToken) =>
        {
            var playerId = user.GetUserId();
            return await playerService.TryJoinRoomAsync(playerId, roomId, cancellationToken);
        });

        roomEndpoints.MapPost("/leave", async (ClaimsPrincipal user, IPlayerService playerService) =>
        {
            var playerId = user.GetUserId();
            return await playerService.TryLeaveRoomAsync(playerId);
        });

        roomEndpoints.MapGet("/state", async (ClaimsPrincipal user, IPlayerService playerService, IRoomInfoService roomInfoService) =>
        {
            var playerId = user.GetUserId();
            var roomId = await playerService.GetCurrentRoomId(playerId);
            return await roomInfoService.GetRoomStateAsync(roomId);
        });

        roomEndpoints.MapGet("/game", async (ClaimsPrincipal user, IPlayerService playerService, IRoomInfoService roomInfoService) =>
        {
            var playerId = user.GetUserId();
            var roomId = await playerService.GetCurrentRoomId(playerId);
            return await roomInfoService.GetGameStateAsync(roomId, playerId);
        });

        roomEndpoints.MapPost("/ready", async (ClaimsPrincipal user, IPlayerService playerService) =>
        {
            var playerId = user.GetUserId();
            await playerService.SetReadyAsync(playerId);
        });

        roomEndpoints.MapPost("/attack", async ([FromBody] Card card, ClaimsPrincipal user, IPlayerService playerService) =>
        {
            var playerId = user.GetUserId();
            await playerService.AttackAsync(playerId, card);
        });

        roomEndpoints.MapPost("/defend", async ([FromBody] DefendPositionCard defendPositionCard, ClaimsPrincipal user, IPlayerService playerService) =>
        {
            var playerId = user.GetUserId();
            await playerService.DefendAsync(playerId, defendPositionCard.AttackCardId, defendPositionCard.DefendCard);
        });

        roomEndpoints.MapPost("/pass", async (ClaimsPrincipal user, IPlayerService playerService) =>
        {
            var playerId = user.GetUserId();
            await playerService.SetPassAsync(playerId);
        });

        roomEndpoints.MapPost("/take", async (ClaimsPrincipal user, IPlayerService playerService) =>
        {
            var playerId = user.GetUserId();
            await playerService.SetWantToTakeAsync(playerId);
        });

        roomEndpoints.MapGet("/current", async (ClaimsPrincipal user, IPlayerService playerService) =>
        {
            var playerId = user.GetUserId();
            return await playerService.GetCurrentRoomId(playerId);
        });
    }
}
