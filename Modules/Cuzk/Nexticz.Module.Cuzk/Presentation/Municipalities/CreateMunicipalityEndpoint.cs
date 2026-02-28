using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Cuzk.Contracts.Municipalities;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Cuzk.Application.Municipalities.Commands.CreateMunicipality;

namespace Nexticz.Module.Cuzk.Presentation.Municipalities;

internal static class CreateMunicipalityEndpoint
{
    public static IEndpointRouteBuilder MapCreateMunicipalityEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(CuzkEndpoints.MunicipalityEndpoints.CreateMunicipality,
                async (
                    CreateMunicipalityRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new CreateMunicipalityCommand(request), 
                            cancellationToken);

                    return result.Match(
                        municipality => Results.Created(
                            $"{CuzkEndpoints.MunicipalityEndpoints.GetMunicipalities}/{municipality.Code}",
                            MunicipalityResponseFactory.Create(municipality)),
                        ResultsHelper.Problem);
                })
            .Produces<MunicipalityResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(CuzkEndpoints.GetOpenApiName(nameof(CuzkEndpoints.MunicipalityEndpoints.CreateMunicipality)));

        return builder;
    }
}