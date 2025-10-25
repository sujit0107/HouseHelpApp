using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HouseHelp.Application.Auth;
using HouseHelp.Contracts.Auth;
using HouseHelp.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HouseHelp.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSecurityTokenHandler _handler = new();
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public TokenResponseDto GenerateTokens(User user)
    {
        return CreateTokens(user);
    }

    public TokenResponseDto RefreshTokens(User user)
    {
        return CreateTokens(user);
    }

    private TokenResponseDto CreateTokens(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_options.AccessMinutes);

        var claims = AuthService.BuildClaims(user).ToList();
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var accessToken = _handler.WriteToken(token);
        var refreshPayload = $"{user.Id}:{Guid.NewGuid()}";
        var refreshToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(refreshPayload));

        return new TokenResponseDto(accessToken, refreshToken, new UserSummaryDto(user.Id, user.Phone, user.Email, user.Name, user.Role));
    }
}
