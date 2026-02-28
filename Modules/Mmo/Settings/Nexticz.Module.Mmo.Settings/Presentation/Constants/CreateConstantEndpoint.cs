using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Constants;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Constants;
using Nexticz.Module.Mmo.Settings.Application.Constants.Commands.CreateConstant;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;


namespace Nexticz.Module.Mmo.Settings.Presentation.Constants;

internal static class CreateConstantEndpoint
{
    public static IEndpointRouteBuilder MapCreateConstant(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.ConstantEndpoints.CreateConstant,
                async (
                    CreateConstantRequest request,
                    ISender mediator,
                    CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(
                        new CreateConstantCommand(request.Key, request.Value, (ConstantType)request.ConstantType, request.Description), 
                        cancellationToken);
        
                    return result.Match(
                        mmoConstant => 
                            Results.Created($"/{SettingsEndpoints.ConstantEndpoints.GetConstantByKey}/{mmoConstant.Key}", 
                                ConstantResponseFactory.Create(mmoConstant)),
                        ResultsHelper.Problem);
                })
            .Produces<ConstantResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ConstantEndpoints.CreateConstant)));

        return builder;
    }
}