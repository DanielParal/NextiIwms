using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Partners.Commands.CreatePartner;
using Nexticz.Module.Vh.Contracts.Partners;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Partners;

public static class CreatePartnersEndpoint
{
    public static IEndpointRouteBuilder MapCreatePartner(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Partners.CreatePartner,
                async (CreatePartnerRequest createPartnerRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new CreatePartnerCommand { CreatePartnerRequest = createPartnerRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Partners.CreatePartner));

        return builder;
    }
}