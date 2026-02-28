using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Settings.Application.ImportsExports.Imports.ImportHandlers;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.ImportsExports.Imports.Commands.CreateImport;

internal class CreateImportCommandHandler (
    ILogger<CreateImportCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISettingsImportHandlerSelector settingsImportHandlerSelector,
    IClock clock
    ) : IRequestHandler<CreateImportCommand, ErrorOr<Import>>
{
    public async Task<ErrorOr<Import>> Handle(CreateImportCommand request, CancellationToken cancellationToken)
    {
        var handler = settingsImportHandlerSelector.GetHandler(request.ImportType);

        if (handler is null)
        {
            logger.LogWarning("No handler found for {ImportSource}", request.ImportType);
            return ImportErrors.ValidationHandlerNotImplementedFor(request.ImportType);
        }
        
        var importBaseResult = await handler.HandleAsync(request.FormFile, cancellationToken);
        var import = Import.CreateFrom(request.UserName, request.ImportType, clock.UtcNowOffset, importBaseResult);
        var importCreatedEvent = new ImportCreatedEvent(import.Id, import.UserName, import.Type, import.Status, import.Errors, import.DateCreated, import.ImportedCodes);
        
        unitOfWork.StartStream<ImportCreatedEvent, Import>(import.Id, importCreatedEvent);
            
        return import;
    }
}