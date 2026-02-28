using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.LoadedActivities.Commands.CreateLoadedActivities;
using Nexticz.Module.Vh.Application.LoadedActivities.PdaReaderRawEvents;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadedActivities;

public static class ImportLoadedActivitiesEndpoint
{
    public static IEndpointRouteBuilder MapImportLoadedActivitiesEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.LoadedActivities.ImportLoadedActivities,
            async (HttpRequest request, ISender mediatr, CancellationToken cancellationToken) =>
            {
                if (!request.HasFormContentType)
                    return Results.Problem("Not multipart/form-data.");
        
                var form = await request.ReadFormAsync(cancellationToken);
        
                var file = form.Files.GetFile("file");
        
                if (file is null)
                    return Results.Problem("No file attached.");

                if (!Enum.TryParse<PdaReaderRawEventsSource>(form["pdaReaderRawEventsSource"], out var sourceType))
                {
                    return Results.Problem("Missing LoadedActivitySource.");
                }
                
                var command = new CreateLoadedActivitiesCommand(new PdaReaderRawEvents(file, sourceType));
                var result = await mediatr.Send(command, cancellationToken);
                
                return result.Match(
                    Results.Ok,
                    ResultsHelper.Problem);
            })
            .Produces(StatusCodes.Status204NoContent)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.LoadedActivities.ImportLoadedActivities))
            .Accepts<IFormCollection>("multipart/form-data");
        
        return builder;
    }
}