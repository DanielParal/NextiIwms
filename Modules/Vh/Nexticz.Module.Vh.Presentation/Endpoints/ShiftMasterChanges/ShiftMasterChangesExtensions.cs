using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.ShiftMasterChanges;

public static class ShiftMasterChangesExtensions
{
    public static IEndpointRouteBuilder MapShiftMasterChangesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetShiftMasterChanges();
    }
}