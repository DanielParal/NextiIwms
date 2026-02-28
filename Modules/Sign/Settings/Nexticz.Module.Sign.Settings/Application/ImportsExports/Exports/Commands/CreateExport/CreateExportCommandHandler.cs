using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Sign.Settings.Application.ImportsExports.Exports.ExportHandlers;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.ExportAggregate;
using Nexticz.Module.Sign.Settings.Domain.ExportAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.ImportsExports.Exports.Commands.CreateExport;

internal class CreateExportCommandHandler(
        ISettingsExportHandlerSelector handlerSelector,
        ILogger<CreateExportCommandHandler> logger,
        ISettingsUnitOfWork unitOfWork,
        IClock clock) 
    : IRequestHandler<CreateExportCommand, ErrorOr<FileResult>>
{
    public async Task<ErrorOr<FileResult>> Handle(CreateExportCommand request, CancellationToken cancellationToken)
    {
        var handler = handlerSelector.GetHandler(request.ExportType);
        
        if (handler is null)
        {
            logger.LogWarning("No handler found for {ExportSource}", request.ExportType);
            return ExportErrors.ValidationHandlerNotImplementedFor(request.ExportType);
        }
        
        var exportResult = await handler.HandleAsync(cancellationToken);
        
        var export = new Export(request.UserName, request.ExportType, clock.UtcNowOffset);
        
        var exportCreatedEvent = new ExportCreatedEvent(export.Id, export.UserName, export.Type, export.DateCreated);
        
        unitOfWork.StartStream<ExportCreatedEvent, Export>(export.Id, exportCreatedEvent);
        
        logger.LogInformation("Sign - Settings - export created with id: {ExportId}.", export.Id);
        
        return exportResult;
    }
}