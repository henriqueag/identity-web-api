using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IdentityWebApi.Models;
using IdentityWebApi.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IdentityWebApi.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public SecurityToken GetSecurityToken(ApplicationUser user, IEnumerable<Claim> userClaims)
    {
        var claims = new List<Claim>
        {
            new Claim("userId", user.Id),
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
        };

        foreach(var userClaim in userClaims) 
        {
            claims.Add(userClaim);
        }

        var securityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddSeconds(_jwtSettings.LifetimeInSeconds),
            signingCredentials: new SigningCredentials(JwtSettings.SecurityKey, SecurityAlgorithms.HmacSha256Signature)
            );

        var tokenHandler = new JwtSecurityTokenHandler();

        return new SecurityToken(tokenHandler.WriteToken(securityToken), _jwtSettings.LifetimeInSeconds);
    }   
}