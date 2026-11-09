using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LiveLearn.Ordering.Application.Services;

namespace LiveLearn.Ordering.API.Services;


internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid UserId => Guid.Parse(
        httpContextAccessor.HttpContext?.User
            .FindFirstValue(JwtRegisteredClaimNames.Sub) ?? Guid.Empty.ToString());
}
