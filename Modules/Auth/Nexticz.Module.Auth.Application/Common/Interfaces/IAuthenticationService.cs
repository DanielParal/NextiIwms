namespace Nexticz.Module.Auth.Application.Common.Interfaces;

public interface IAuthenticationService
{
    void AddResponseAuthorizationCookies(string accessToken, string refreshToken);
    void DeleteResponseAuthorizationCookies();
}