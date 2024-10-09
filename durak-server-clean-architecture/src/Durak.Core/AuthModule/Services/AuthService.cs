using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Durak.Core.AuthModule.Constants;
using Durak.Core.AuthModule.Exceptions;
using Durak.Core.AuthModule.Services.IServices;
using Durak.Core.Common.Helpers;
using Durak.Core.Common.Options;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;

namespace Durak.Core.AuthModule.Services;

public class AuthService : IAuthService
{
    private readonly TimeSpan _jwtExpirationDuration = TimeSpan.FromHours(1);
    private readonly string _jwtSecretKey;
    private readonly byte[] _telegramSecretKey;
    private readonly TimeSpan _telegramTokenExpirationDuration = TimeSpan.FromDays(1);

    public AuthService(BotOptions botOptions, AuthOptions authOptions)
    {
        var data = Encoding.UTF8.GetBytes(botOptions.BotToken);
        var key = Encoding.UTF8.GetBytes(AuthConstants.TelegramKeySalt);
        _telegramSecretKey = HMACSHA256.HashData(key, data);
        _jwtSecretKey = authOptions.JwtSecretKey;
    }

    public string Authenticate(string telegramToken)
    {
        ValidateTelegramToken(telegramToken);
        return GenerateJwtToken(telegramToken);
    }

    private string GenerateJwtToken(string telegramToken)
    {
        var claims = ExtractClaimsFromTelegramToken(telegramToken);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.Add(_jwtExpirationDuration),
            signingCredentials: signingCredentials);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

    private void ValidateTelegramToken(string telegramToken)
    {
        if (string.IsNullOrEmpty(telegramToken))
            throw new ArgumentException("Telegram token is empty");

        var searchParams = QueryHelpers.ParseQuery(telegramToken);

        if (searchParams.Count == 0)
            throw new ArgumentException("Params in token are missed");

        string? hash = null;
        DateTime? authDate = null;
        var pairs = new List<string>(searchParams.Count - 1);

        foreach (var param in searchParams)
        {
            switch (param.Key)
            {
                case TelegramPropertyConstants.HashParam:
                    hash = param.Value.ToString();
                    continue;
                case TelegramPropertyConstants.AuthDateParam:
                {
                    if (!int.TryParse(param.Value.ToString(), out var authDateNum))
                        throw new Exception("Failed to parse auth date");

                    authDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                        .AddSeconds(authDateNum);
                    break;
                }
            }

            pairs.Add($"{param.Key}={param.Value}");
        }

        Guard.IsNullOrEmpty(hash, nameof(hash));
        var validAuthDate = Guard.EnsureValue(authDate, nameof(authDate));

        if (validAuthDate.Add(_telegramTokenExpirationDuration) < DateTime.UtcNow)
            throw new ExpiredTokenException();

        pairs.Sort();
        var computedHash = BitConverter
            .ToString(HMACSHA256.HashData(_telegramSecretKey, Encoding.UTF8.GetBytes(string.Join("\n", pairs))))
            .Replace("-", "").ToLower();

        if (hash != computedHash)
            throw new InvalidHashException();
    }

    private static List<Claim> ExtractClaimsFromTelegramToken(string token)
    {
        var claims = new List<Claim>();
        var searchParams = QueryHelpers.ParseQuery(token);

        if (!searchParams.TryGetValue(TelegramPropertyConstants.User, out var userData))
            return claims;

        using var document = JsonDocument.Parse(userData.ToString());
        var root = document.RootElement;

        var isUserIdExist = root.TryGetProperty(TelegramPropertyConstants.Id, out var userIdData);
        var userId = userIdData.GetInt64();
        if (!isUserIdExist || userId == default)
            throw new UserIdNotFoundException();
        claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

        var isFirstNameExist = root.TryGetProperty(TelegramPropertyConstants.FirstName, out var userNameData);
        if (!isFirstNameExist)
            return claims;
        var firstName = userNameData.GetString() ?? AuthConstants.DefaultFirstName;
        claims.Add(new Claim(ClaimTypes.Name, firstName));

        return claims;
    }
}