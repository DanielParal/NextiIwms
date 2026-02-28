using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Washing.Presentation.Batches;

internal static class BatchesEndpointsExtensions
{
    public static IEndpointRouteBuilder MapBatchesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapFinishKitEndpoint()
            .MapGetBatchByLineCodeEndpoint()
            .MapCreatePrintingEndpoint()
            .MapGetKitInstructionFileEndpoint()
            .MapConfirmSpecialInformationEndpoint()
            .MapGetSpecialInformationFileEndpoint();
    }
}