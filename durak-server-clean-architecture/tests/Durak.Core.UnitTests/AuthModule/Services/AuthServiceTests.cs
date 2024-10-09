using System.Security.Cryptography;
using System.Text;
using Durak.Core.AuthModule.Constants;
using Durak.Core.AuthModule.Services;
using Durak.Core.Common.Options;
using Microsoft.AspNetCore.WebUtilities;

namespace Durak.Core.UnitTests.AuthModule.Services;

public class AuthServiceTests
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private readonly BotOptions _botOptions;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _botOptions = new BotOptions
        {
            BotToken = GenerateRandomString(20)
        };
        
        var authOptions = new AuthOptions
        {
            JwtSecretKey = GenerateRandomString(64)
        };
        
        _authService = new AuthService(_botOptions, authOptions);
    }

    [Fact]
    public void Authenticate_ValidTelegramToken_ShouldReturnJwt()
    {
        // Arrange
        var token = GenerateToken(_botOptions.BotToken);

        // Act
        var result = _authService.Authenticate(token);

        //Assert
        Assert.NotEmpty(result);
    }

    private string GenerateToken(string botToken)
    {
        var token = "query_id=AAHdF6IQAAAAAN0XohDhrOrc" +
                    "&user=%7B%22id%22%3A279058397%2C%22" +
                    "first_name%22%3A%22John%22%2C%22" +
                    "last_name%22%3A%22Doe%22%2C%22" +
                    "username%22%3A%22john_doe%22%2C%22" +
                    "language_code%22%3A%22en%22%2C%22" +
                    "is_premium%22%3Atrue%7D&" +
                    "auth_date=1772771648";
        
        var data = Encoding.UTF8.GetBytes(botToken);
        var key = Encoding.UTF8.GetBytes(AuthConstants.TelegramKeySalt);
        var telegramSecretKey = HMACSHA256.HashData(key, data);
        
        var searchParams = QueryHelpers.ParseQuery(token);

        var pairs = new List<string>(searchParams.Count - 1);
        pairs.AddRange(searchParams.Select(param => $"{param.Key}={param.Value}"));

        pairs.Sort();
        var computedHash = BitConverter
            .ToString(HMACSHA256.HashData(telegramSecretKey, Encoding.UTF8.GetBytes(string.Join("\n", pairs))))
            .Replace("-", "").ToLower();

        token += $"&hash={computedHash}";
        
        return token;
    }

    private static string GenerateRandomString(int length)
    {
        var random = new Random();
        var stringChars = new char[length];
        for (var i = 0; i < length; i++)
        {
            stringChars[i] = Chars[random.Next(Chars.Length)];
        }
        return new string(stringChars);
    }
}