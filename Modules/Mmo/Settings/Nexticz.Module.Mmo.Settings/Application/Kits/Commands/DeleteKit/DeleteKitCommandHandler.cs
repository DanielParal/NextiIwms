using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.FileHandling;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitByCode;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Commands.DeleteKit;

internal class DeleteKitCommandHandler (
    ISender sender, 
    ISettingsUnitOfWork unitOfWork,
    ILogger<DeleteKitCommandHandler> logger,
    ISettingsFileHandler fileHandler)
    : IRequestHandler<DeleteKitCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteKitCommand request, CancellationToken cancellationToken)
    {
        var kit = await sender.Send(new GetKitByCodeQuery(request.Code), cancellationToken);

        if (kit.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with Code: {Code}. Nothing to delete.", nameof(Kit), request.Code);
            return kit.Errors;
        }

        fileHandler.DeleteKitInstructionPdf(kit.Value.Code);
        
        var kitDeletedEvent = new KitDeletedEvent(kit.Value.Id, request.Code);
        unitOfWork.AppendEvent(kit.Value.Id, kitDeletedEvent);
        return Result.Deleted;
    }
}