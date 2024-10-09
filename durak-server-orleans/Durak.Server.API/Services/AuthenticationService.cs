using Durak.Server.API.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Durak.Server.API.Services;

public class AuthenticationService : IAuthenticationService
{
    private static byte[] _telegramSecretKey = [];
    private readonly string _botToken;
    private readonly string _jwtSecretKey;
    private readonly TimeSpan _expirationDuration = TimeSpan.FromDays(1);

    public AuthenticationService(IConfiguration configuration)
    {
        _botToken = configuration.GetValue<string>("BOT_TOKEN")!;
        var data = Encoding.UTF8.GetBytes(_botToken);
        var key = Encoding.UTF8.GetBytes("WebAppData");
        _telegramSecretKey = HMACSHA256.HashData(key, data);
        _jwtSecretKey = configuration.GetValue<string>("SECRET_KEY")!;
    }

    public string Authenticate(string telegramToken, out long playerId, out string playerName)
    {
        playerName = "Player";
        if (IsTelegramTokenValid(telegramToken, out var userId, out var userName))
        {
            playerName = userName;
            playerId = userId;
            return GenerateJwt(userId, userName);
        }
        else
        {
            throw new Exception("Invalid token");
        }
    }

    public string GenerateJwt(long userId, string userName)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, userName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
                               claims: claims,
                               expires: DateTime.UtcNow.AddDays(1),
                               signingCredentials: signingCredentials);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

    public bool IsTelegramTokenValid(string token, out long userId, out string userName)
    {
        userId = 0;
        userName = "Player";

        var searchParams = QueryHelpers.ParseQuery(token);

        if (searchParams.Count == 0)
            return false;

        var authDate = DateTime.MinValue;
        var hash = string.Empty;
        var pairs = new List<string>(searchParams.Count - 1);

        foreach (var param in searchParams)
        {
            if (param.Key == "hash")
            {
                hash = param.Value.ToString();
                continue;
            }

            if (param.Key == "auth_date")
            {
                if (!int.TryParse(param.Value.ToString(), out var authDateNum))
                {
                    return false;
                }

                authDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    .AddSeconds(authDateNum);
            }

            pairs.Add($"{param.Key}={param.Value}");
        }

        if (string.IsNullOrEmpty(hash))
        {
            return false;
        }

        if (authDate == DateTime.MinValue)
        {
            return false;
        }

        if (authDate.Add(_expirationDuration) < DateTime.UtcNow)
        {
            return false;
        }

        pairs.Sort();
        var computedHash = BitConverter.ToString(HMACSHA256.HashData(_telegramSecretKey, Encoding.UTF8.GetBytes(string.Join("\n", pairs)))).Replace("-", "").ToLower();

        if (computedHash == hash)
        {
            searchParams.TryGetValue("user", out var userData);

            if (JsonDocument.Parse(userData.ToString()).RootElement.TryGetProperty("id", out var userIdData))
            {
                userId = userIdData.GetInt64();
            }

            if (JsonDocument.Parse(userData.ToString()).RootElement.TryGetProperty("first_name", out var userNameData))
            {
                userName = userNameData.GetString() ?? "Player";
            }
        }

        return computedHash == hash;
    }
}
