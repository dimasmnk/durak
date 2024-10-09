using System.Security.Claims;

namespace Durak.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static long GetUserId(this ClaimsPrincipal claims)
    {
        return long.Parse(claims.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    public static string GetUsername(this ClaimsPrincipal claims)
    {
        return claims.FindFirstValue(ClaimTypes.Name)!;
    }
}
