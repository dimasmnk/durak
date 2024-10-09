using Durak.Application.AuthModule.Commands;
using Durak.Application.AuthModule.Responses;
using Durak.Core.AuthModule.Services.IServices;
using MediatR;

namespace Durak.Application.AuthModule.Handlers;

public class AuthenticateUserCommandHandler(IAuthService authService)
    : IRequestHandler<AuthenticateUserCommand, AuthenticateUserResponse>
{
    private readonly IAuthService _authService = authService;

    public Task<AuthenticateUserResponse> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        var accessToken = _authService.Authenticate(request.TelegramToken);
        return Task.FromResult(new AuthenticateUserResponse(accessToken));
    }
}