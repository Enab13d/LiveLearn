using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace LiveLearn.Catalog.Infrastructure.Services;


internal sealed class JwtWatchTicketSigner : IWatchTicketSigner, IDisposable
{
    private readonly WatchTicketOptions _options;
    private readonly TimeProvider _timeProvider;
    private readonly ECDsa _key;
    private readonly SigningCredentials _signingCredentials;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtWatchTicketSigner(IOptions<WatchTicketOptions> options, TimeProvider timeProvider)
    {
        _options = options.Value;
        _timeProvider = timeProvider;
        _key = ECDsa.Create();
        _key.ImportPkcs8PrivateKey(Convert.FromBase64String(_options.PrivateKey), out _);
        _signingCredentials = new SigningCredentials(new ECDsaSecurityKey(_key), SecurityAlgorithms.EcdsaSha256);
    }

    public WatchTicketDto IssueWatchTicket(Guid userId, Guid lectureId, Guid videoId)
    {
        var now = _timeProvider.GetUtcNow();
        var expiresAt = now.AddMinutes(_options.TtlMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim("sub", userId.ToString()),
                new Claim("lectureId", lectureId.ToString()),
                new Claim("videoId", videoId.ToString())
            ]),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = now.UtcDateTime,
            NotBefore = now.UtcDateTime,
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = _signingCredentials
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);

        return new WatchTicketDto(_tokenHandler.WriteToken(token), videoId, expiresAt);
    }

    public void Dispose() => _key.Dispose();
}
