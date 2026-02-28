using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Queries.GetKitSapDefinitionByCode;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Commands.UpdateKitSapDefinition;

internal class UpdateKitSapDefinitionCommandHandler(
    ILogger<UpdateKitSapDefinitionCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender
) : IRequestHandler<UpdateKitSapDefinitionCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateKitSapDefinitionCommand request, CancellationToken cancellationToken)
    {
        var kitSapDefinition = await sender.Send(new GetKitSapDefinitionByCodeQuery(request.Code), cancellationToken);
        
        if (!kitSapDefinition.HasValue())
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to update.", nameof(KitSapDefinition), request.Code);
            return KitSapDefinitionErrors.CodeDoesNotExist;
        }
        
        var kitSapDefinitionNameUpdatedEvent = new KitSapDefinitionNameUpdatedEvent(kitSapDefinition.Value.Id, request.Name);

        unitOfWork.AppendEvent(kitSapDefinition.Value.Id, kitSapDefinitionNameUpdatedEvent);

        return Result.Updated;
    }
}