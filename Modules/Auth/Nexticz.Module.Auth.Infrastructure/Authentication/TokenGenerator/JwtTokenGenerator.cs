using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nexticz.Module.Auth.Application.Common.Interfaces;
using Nexticz.Module.Auth.Domain.AppUserClaims;
using Nexticz.Module.Auth.Domain.AppUserRoles;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Auth.Infrastructure.Authentication.Configurations;

namespace Nexticz.Module.Auth.Infrastructure.Authentication.TokenGenerator;

public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>() 
                                                ?? throw new InvalidOperationException("Jwt Settings not found.");

    private SymmetricSecurityKey Key => new(Encoding.UTF8.GetBytes(_jwtSettings.TokenSecretKey));

    public string GenerateAccessToken(AppUser appUser)
    {
        var credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha512);

        var claims = new List<Claim>
        {
            new(StringHelper.Claim.Type.MagicUniqueName, appUser.UserName ?? ""),
            new(StringHelper.Claim.Type.MagicFirstName, appUser.Firstname ?? ""),
            new(StringHelper.Claim.Type.MagicFamilyName, appUser.Lastname ?? ""),
            new(StringHelper.Claim.Type.MagicEmail, appUser.Email ?? ""),
            new(StringHelper.Claim.Type.MagicId, appUser.Id.ToString())
        };

        var userRoles = appUser.AppUserRoles?.ToList();
        AddRoles(userRoles, claims);

        var userPermissions = appUser.AppUserClaims?.ToList();
        AppPermissions(userPermissions, claims);

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.Now.AddMinutes(double.Parse(_jwtSettings.TokenExpirationTime)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateAccessTokenForApiKey(AppUser appUser)
    {
        var credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha512);

        var claims = new List<Claim>
        {
            new(StringHelper.Claim.Type.MagicUniqueName, appUser.UserName ?? ""),
            new(StringHelper.Claim.Type.MagicFirstName, appUser.Firstname ?? ""),
            new(StringHelper.Claim.Type.MagicFamilyName, appUser.Lastname ?? ""),
            new(StringHelper.Claim.Type.MagicEmail, appUser.Email ?? ""),
            new(StringHelper.Claim.Type.MagicId, appUser.Id.ToString())
        };

        var userRoles = appUser.AppUserRoles?.ToList();
        AddRoles(userRoles, claims);

        var userPermissions = appUser.AppUserClaims?.ToList();
        AppPermissions(userPermissions, claims);

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.Now.AddMinutes(double.Parse(_jwtSettings.TokenExpirationTime)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? ValidateAccessToken(string token, bool validateLifeTime)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principals =
                tokenHandler.ValidateToken(token, GetAccessTokenValidationParameters(validateLifeTime), out _);

            return principals;
        }

        catch
        {
            return null;
        }
    }

    public JwtSecurityToken? DecodeJwtToken(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        return handler.ReadToken(jwtToken) as JwtSecurityToken;
    }

    private TokenValidationParameters GetAccessTokenValidationParameters(bool validateLifeTime)
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtSettings.TokenSecretKey)),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = validateLifeTime,
            ClockSkew = TimeSpan.Zero
        };
    }

    private static void AddRoles(List<AppUserRole>? userRoles, List<Claim> claims)
    {
        userRoles?.ForEach(appUserRole =>
        {
            claims.Add(new Claim(StringHelper.Claim.Type.MagicRoles, appUserRole.AppRole?.Name!));
        });
    }

    private static void AppPermissions(List<AppUserClaim>? userRoles, List<Claim> claims)
    {
        userRoles?.ForEach(appUserClaim =>
        {
            claims.Add(new Claim(StringHelper.Claim.Type.MagicPermissions, appUserClaim.ClaimValue!));
        });
    }
}