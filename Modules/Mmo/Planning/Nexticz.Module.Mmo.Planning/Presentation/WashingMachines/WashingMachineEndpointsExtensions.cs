using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Planning.Presentation.WashingMachines;

internal static class WashingMachineEndpointsExtensions
{
    public static IEndpointRouteBuilder MapWashingMachineEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateBatchEndpoint()
            .MapRemoveBatchEndpoint()
            .MapChangeBatchKitsCountEndpoint()
            .MapMoveBatchInQueueEndpoint()
            .MapActivateBatchEndpoint()
            .MapFinishBatchEndpoint()
            .MapSplitBatchEndpoint()
            .MapDetachSisterBatchesEndpoint()
            .MapMoveBatchToAnotherQueueEndpoint();
    }
    
    public static IEndpointRouteBuilder MapGetWashingMachineEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetWashingMachinesEndpoint();
    }
}