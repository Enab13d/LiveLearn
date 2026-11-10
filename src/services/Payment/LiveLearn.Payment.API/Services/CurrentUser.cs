using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LiveLearn.Payment.Application.Services;

namespace LiveLearn.Payment.API.Services;


internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid UserId => Guid.Parse(
        httpContextAccessor.HttpContext?.User
            .FindFirstValue(JwtRegisteredClaimNames.Sub) ?? Guid.Empty.ToString());
}
