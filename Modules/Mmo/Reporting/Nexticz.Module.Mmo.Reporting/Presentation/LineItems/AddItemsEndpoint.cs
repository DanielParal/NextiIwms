using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Reporting.Contracts.LineItems;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Commands.AddItems;
using Nexticz.Module.Mmo.Reporting.Domain.Views;


namespace Nexticz.Module.Mmo.Reporting.Presentation.LineItems;

internal static class AddItemsEndpoint
{
    public static IEndpointRouteBuilder MapAddItemsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ReportingEndpoints.LineItemEndpoints.AddItems,
                async (
                    AddItemsRequest request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(new AddItemsCommand(
                            request.ShiftId, request.StartDate, request.EndDate, (AddLineItemType)request.Type, request.LineCodes), 
                            cancellationToken);
                    
                    return result.Match(
                        _ => Results.Ok(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(ReportingEndpoints.GetOpenApiName(nameof(ReportingEndpoints.LineItemEndpoints.AddItems)));

        return builder;
    }
}