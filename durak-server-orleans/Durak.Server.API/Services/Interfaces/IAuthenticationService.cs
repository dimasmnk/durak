namespace Durak.Server.API.Services.Interfaces;

public interface IAuthenticationService
{
    string Authenticate(string telegramToken, out long playerId, out string playerName);
}