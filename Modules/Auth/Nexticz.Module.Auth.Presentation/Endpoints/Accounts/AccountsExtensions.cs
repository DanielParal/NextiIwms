using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Auth.Presentation.Endpoints.Accounts;

public static class AccountsExtensions
{
    public static IEndpointRouteBuilder MapAccountsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGetAllAccounts();
        builder.MapGetAccountsByUsernameOrId();
        builder.MapCreateAccount();
        builder.MapUpdateAccount();
        builder.MapDeleteAccount();
        builder.MapSyncAccountsInModules();

        return builder;
    }
}