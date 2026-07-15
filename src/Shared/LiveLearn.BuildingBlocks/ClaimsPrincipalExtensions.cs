using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace LiveLearn.BuildingBlocks;


public static class ClaimsPrincipalExtensions
{

    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {

        var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        return Guid.TryParse(sub, out var id) ? id : null;
    }
}
