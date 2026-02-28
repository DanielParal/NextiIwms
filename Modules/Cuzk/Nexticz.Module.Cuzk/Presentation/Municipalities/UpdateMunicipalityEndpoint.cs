using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Cuzk.Contracts.Municipalities;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Cuzk.Application.Municipalities.Commands.UpdateModule;

namespace Nexticz.Module.Cuzk.Presentation.Municipalities;

internal static class UpdateMunicipalityEndpoint
{
    public static IEndpointRouteBuilder MapUpdateMunicipalityEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(CuzkEndpoints.MunicipalityEndpoints.UpdateMunicipality,
                async (
                    string code, 
                    UpdateMunicipalityRequest request, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new UpdateMunicipalityCommand(code, request), cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(CuzkEndpoints.GetOpenApiName(nameof(CuzkEndpoints.MunicipalityEndpoints.UpdateMunicipality)));
        
        return builder;
    }
}