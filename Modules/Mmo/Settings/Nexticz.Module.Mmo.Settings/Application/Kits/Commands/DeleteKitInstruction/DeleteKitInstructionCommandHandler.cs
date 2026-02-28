using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.FileHandling;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitByCode;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Commands.DeleteKitInstruction;

internal class DeleteKitInstructionCommandHandler(ISender sender,
    ILogger<DeleteKitInstructionCommandHandler> logger,
    ISettingsFileHandler fileHandler,
    ISettingsUnitOfWork unitOfWork) : IRequestHandler<DeleteKitInstructionCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DeleteKitInstructionCommand request, CancellationToken cancellationToken)
    {
        var kit = await sender.Send(new GetKitByCodeQuery(request.KitCode), cancellationToken);
        if (kit.IsError)
        {
            logger.LogWarning("Settings - Did not find object {ObjectName} with Code: {Code}. Cannot delete kit instruction.", nameof(Kit), request.KitCode);
            return KitErrors.ValidationKitDoesNotExist;
        }

        fileHandler.DeleteKitInstructionPdf(request.KitCode);
        
        var kitInstructionDeletedEvent = new KitInstructionDeletedEvent(kit.Value.Id, kit.Value.Code);
        unitOfWork.AppendEvent(kit.Value.Id, kitInstructionDeletedEvent);
        
        return Result.Success;
    }
}