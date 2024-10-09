using System.Security.Claims;

namespace Durak.Server.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static long GetUserId(this ClaimsPrincipal claims)
    {
        return long.Parse(claims.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    public static string GetUserName(this ClaimsPrincipal claims)
    {
        return claims.FindFirstValue(ClaimTypes.Name)!;
    }
}
