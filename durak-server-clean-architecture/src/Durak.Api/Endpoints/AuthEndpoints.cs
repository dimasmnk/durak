using Durak.Application.AuthModule.Commands;
using MediatR;

namespace Durak.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var userEndpoints = app.MapGroup("/auth");

        userEndpoints.MapPost("/token",
            async (AuthenticateUserCommand authenticateUserCommand, IMediator mediator, CancellationToken cancellationToken)
                => await mediator.Send(authenticateUserCommand, cancellationToken));
    }
}