using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LiveLearn.Identity.Application.Services;

namespace LiveLearn.Identity.API.Services;


internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid UserId => Guid.Parse(Claim(JwtRegisteredClaimNames.Sub) ?? Guid.Empty.ToString());

    public string Email => Claim(JwtRegisteredClaimNames.Email) ?? "";

    public string FirstName => Claim(JwtRegisteredClaimNames.GivenName) ?? "";

    public string LastName => Claim(JwtRegisteredClaimNames.FamilyName) ?? "";

    private string? Claim(string type) => httpContextAccessor.HttpContext?.User.FindFirstValue(type);
}
