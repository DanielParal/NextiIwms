using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.DepositorsGroups;

public static class DepositorsGroupsExtensions
{
    public static IEndpointRouteBuilder MapDepositorsGroupsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder
            .MapCreateDepositorsGroup()
            .MapUpdateDepositorsGroup()
            .MapDeleteDepositorsGroup()
            .MapGetDepositorsGroupById()
            .MapGetDepositorsGroups();

        return builder;
    }
}