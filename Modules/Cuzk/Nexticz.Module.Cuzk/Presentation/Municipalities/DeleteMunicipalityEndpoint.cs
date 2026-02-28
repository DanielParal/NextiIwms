using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Cuzk.Application.Municipalities.Commands.DeleteModule;

namespace Nexticz.Module.Cuzk.Presentation.Municipalities;

internal static class DeleteMunicipalityEndpoint
{
    public static IEndpointRouteBuilder MapDeleteMunicipalityEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(CuzkEndpoints.MunicipalityEndpoints.DeleteMunicipality,
                async (
                    string code, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new DeleteMunicipalityCommand(code), cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(CuzkEndpoints.GetOpenApiName(nameof(CuzkEndpoints.MunicipalityEndpoints.DeleteMunicipality)));

        return builder;
    }
}