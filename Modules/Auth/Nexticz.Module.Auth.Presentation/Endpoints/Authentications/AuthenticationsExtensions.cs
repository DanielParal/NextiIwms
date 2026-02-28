using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Auth.Presentation.Endpoints.Authentications;

public static class AuthenticationsExtensions
{
    public static IEndpointRouteBuilder MapAuthenticationsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapConfirmEmailCompletion();
        builder.MapConfirmEmail();
        builder.MapGetAccountInfo();
        builder.MapLogin();
        builder.MapLoginPasswordless();
        builder.MapLogout();
        builder.MapRefreshAccessToken();
        builder.MapRegister();
        builder.MapRegisterPasswordless();
        builder.MapResetPassword();
        builder.MapGetLoggedUserInfo();
        builder.MapForgottenPassword();

        return builder;
    }
}