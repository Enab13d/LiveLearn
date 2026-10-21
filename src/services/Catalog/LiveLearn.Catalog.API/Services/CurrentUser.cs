using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LiveLearn.Catalog.Application.Services;

namespace LiveLearn.Catalog.API.Services;


internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid UserId => Guid.Parse(
        httpContextAccessor.HttpContext?.User
            .FindFirstValue(JwtRegisteredClaimNames.Sub) ?? Guid.Empty.ToString());

    public bool IsAdmin => httpContextAccessor.HttpContext?.User.IsInRole("Admin") == true;
}
