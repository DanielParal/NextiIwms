using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Partners.Commands.UpdatePartner;
using Nexticz.Module.Vh.Contracts.Partners;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Partners;

public static class UpdatePartnerEndpoint
{
    public static IEndpointRouteBuilder MapUpdatePartner(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.Partners.UpdatePartner,
                async (Guid id, UpdatePartnerRequest updatePartnerRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdatePartnerCommand { Id = id, UpdatePartnerRequest = updatePartnerRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Partners.UpdatePartner));

        return builder;
    }
}