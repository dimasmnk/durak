using Durak.Server.API.Endpoints.Requests;
using Durak.Server.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Durak.Server.API.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        app.MapPost("/auth", async ([FromBody] TelegramToken telegramToken, IAuthenticationService authenticationService, IPlayerService playerService) =>
        {
            var token = authenticationService.Authenticate(telegramToken.Token, out var playerId, out var playerName);
            await playerService.SetPlayerNameAsync(playerId, playerName);
            return token;
        });
    }
}