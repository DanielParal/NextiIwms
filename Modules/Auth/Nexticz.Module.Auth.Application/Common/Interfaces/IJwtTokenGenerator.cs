using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Nexticz.Module.Auth.Domain.AppUsers;

namespace Nexticz.Module.Auth.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(AppUser appUser);
    string GenerateAccessTokenForApiKey(AppUser appUser);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateAccessToken(string token, bool validateLifeTime);
    JwtSecurityToken? DecodeJwtToken(string jwtToken);
}