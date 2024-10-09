using Durak.Application.AuthModule.Responses;
using MediatR;

namespace Durak.Application.AuthModule.Commands;

public abstract class AuthenticateUserCommand(string telegramToken) : IRequest<AuthenticateUserResponse>
{
    public string TelegramToken { get; set; } = telegramToken;
}