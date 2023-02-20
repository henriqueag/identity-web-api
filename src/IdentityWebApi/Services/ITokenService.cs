using System.Security.Claims;
using IdentityWebApi.Models;

namespace IdentityWebApi.Services;

public interface ITokenService
{
    SecurityToken GetSecurityToken(ApplicationUser user, IEnumerable<Claim> userClaims);
}