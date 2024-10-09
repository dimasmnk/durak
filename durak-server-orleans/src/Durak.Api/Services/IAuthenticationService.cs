namespace Durak.Api.Services;

public interface IAuthenticationService
{
    string Authenticate(string telegramToken);
    string GenerateJwt(long userId, string userName);
    bool IsTelegramTokenValid(string token, out long userId, out string userName);
}