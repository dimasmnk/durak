namespace Durak.Application.AuthModule.Responses;

public class AuthenticateUserResponse(string accessToken)
{
    public string AccessToken { get; set; } = accessToken;
}