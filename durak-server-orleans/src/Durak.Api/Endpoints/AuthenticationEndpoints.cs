using Durak.Api.Contracts.Requests;
using Durak.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Durak.Api.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        app.MapPost("/auth", ([FromBody] AuthenticationRequest telegramToken, IAuthenticationService authenticationService) =>
        {
            var token = authenticationService.Authenticate(telegramToken.Token);
            return token;
        });
    }
}
