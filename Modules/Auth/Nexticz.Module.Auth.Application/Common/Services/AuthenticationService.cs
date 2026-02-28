using Microsoft.AspNetCore.Http;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Auth.Application.Common.Interfaces;

namespace Nexticz.Module.Auth.Application.Common.Services;

public class AuthenticationService(IHttpContextAccessor httpContextAccessor) : IAuthenticationService
{
    public void AddResponseAuthorizationCookies(string? accessToken, string? refreshToken)
    {
        var httpContext = httpContextAccessor.HttpContext!;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            Secure = true
        };

        if (accessToken is not null) 
            httpContext.Response.Cookies.Append(StringHelper.Header.XAccessToken, accessToken, cookieOptions);
        
        if (refreshToken is not null)
            httpContext.Response.Cookies.Append(StringHelper.Header.XRefreshToken, refreshToken, cookieOptions);
    }

    public void DeleteResponseAuthorizationCookies()
    {
        var httpContext = httpContextAccessor.HttpContext!;
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Secure = true
        };
        
        httpContext.Response.Cookies.Delete(StringHelper.Header.XAccessToken, cookieOptions);
        httpContext.Response.Cookies.Delete(StringHelper.Header.XRefreshToken, cookieOptions);
    }
}