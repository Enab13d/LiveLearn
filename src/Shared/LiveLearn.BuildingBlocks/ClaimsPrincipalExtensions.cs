using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace LiveLearn.BuildingBlocks;


public static class ClaimsPrincipalExtensions
{

    public static string? GetUserId(this ClaimsPrincipal principal)
    {

        return principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    }
}
