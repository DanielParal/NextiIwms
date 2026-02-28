using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.Settings.Application.ImportsExports.Imports.ImportHandlers;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.ImportAggregate;
using Nexticz.Module.Sign.Settings.Domain.ImportAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.ImportsExports.Imports.Commands.CreateImport;

internal class CreateImportCommandHandler (
    ILogger<CreateImportCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISettingsImportHandlerSelector settingsImportHandlerSelector,
    IClock clock,
    ICurrentUserProvider currentUserProvider
    ) : IRequestHandler<CreateImportCommand, ErrorOr<Import>>
{
    public async Task<ErrorOr<Import>> Handle(CreateImportCommand request, CancellationToken cancellationToken)
    {
        var handler = settingsImportHandlerSelector.GetHandler(request.ImportType);

        if (handler is null)
        {
            logger.LogWarning("Sign - Settings - import - No handler found for {ImportSource}", request.ImportType);
            return ImportErrors.ValidationHandlerNotImplementedFor(request.ImportType);
        }
        
        var userName = currentUserProvider.GetCurrentUser().UserName;
        var importBaseResult = await handler.HandleAsync(request.FormFile, cancellationToken);

        var import = Import.CreateFrom(userName, request.ImportType, clock.UtcNowOffset, importBaseResult);
        var importCreatedEvent = new ImportCreatedEvent(import.Id, import.UserName, import.Type, import.Status, import.Errors, import.DateCreated, import.ImportedCodes);
        
        unitOfWork.StartStream<ImportCreatedEvent, Import>(import.Id, importCreatedEvent);
            
        logger.LogInformation("Sign - Settings - data were imported. ImportType: {ImportType}, ImportId: {ImportId}.",
            import.Type, import.Id);
        
        return import;
    }
}