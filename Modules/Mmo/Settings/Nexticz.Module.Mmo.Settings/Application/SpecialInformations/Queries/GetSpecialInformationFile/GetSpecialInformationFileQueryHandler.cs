using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.FileHandling;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationFile;

internal class GetSpecialInformationFileQueryHandler(
    ISettingsFileHandler fileHandler,
    ILogger<GetSpecialInformationFileQueryHandler> logger) 
    : IRequestHandler<GetSpecialInformationFileQuery, ErrorOr<SpecialInformationFileResult>>
{
    public async Task<ErrorOr<SpecialInformationFileResult>> Handle(GetSpecialInformationFileQuery request, CancellationToken cancellationToken)
    {
        var fileResult = await fileHandler.GetSpecialInformationFileAsync(request.Id, cancellationToken);

        if (fileResult is null)
        {
            logger.LogWarning("Settings - Special information file not found. Id: {Id}", request.Id);
            return SpecialInformationErrors.NotFoundSpecialInformationFile;
        }
        
        return new SpecialInformationFileResult(
            fileResult.ContentBytes, fileResult.ContentType, fileResult.FileName);
    }
}