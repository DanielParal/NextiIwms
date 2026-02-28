using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsByKitSapDefinitionCode;
using Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Queries.GetKitSapDefinitionByCode;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Commands.DeleteKitSapDefinition;

internal class DeleteKitSapDefinitionCommandHandler(
    ILogger<DeleteKitSapDefinitionCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender
) : IRequestHandler<DeleteKitSapDefinitionCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteKitSapDefinitionCommand request, CancellationToken cancellationToken)
    {
        var kitSapDefinition = await sender.Send(new GetKitSapDefinitionByCodeQuery(request.Code), cancellationToken);

        if (kitSapDefinition.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to delete.", 
                nameof(KitSapDefinition), request.Code);
            return kitSapDefinition.Errors;
        }
        
        var existingKitsWithKitSapDefinitionCode = await sender.Send(new GetKitsByKitSapDefinitionCodeQuery(request.Code), cancellationToken);
        
        if (existingKitsWithKitSapDefinitionCode.Any())
        {
            var codes = string.Join(", ", existingKitsWithKitSapDefinitionCode.Select(x => x.Code));
            logger.LogInformation("{ObjectName} is still used inside kits with codes: {Codes}", nameof(KitSapDefinition), codes);
            return KitSapDefinitionErrors.ValidationCodeIsStillUsedInKits(codes);
        }

        var kitSapDefinitionDeletedEvent = new KitSapDefinitionDeletedEvent(kitSapDefinition.Value.Id, request.Code);

        unitOfWork.AppendEvent(kitSapDefinition.Value.Id, kitSapDefinitionDeletedEvent);

        return Result.Deleted;
    }
}