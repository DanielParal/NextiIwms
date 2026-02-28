using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel;
using ErrorOr;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Queries.GetKitSapDefinitionByCode;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Commands.CreateKitSapDefinition;

internal class CreateKitSapDefinitionCommandHandler(
    ILogger<CreateKitSapDefinitionCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender)
    : IRequestHandler<CreateKitSapDefinitionCommand, ErrorOr<KitSapDefinition>>
{
    public async Task<ErrorOr<KitSapDefinition>> Handle(CreateKitSapDefinitionCommand request, CancellationToken cancellationToken)
    {
        var existingKitSapDefinition = await sender.Send(new GetKitSapDefinitionByCodeQuery(request.Code), cancellationToken);
        if (existingKitSapDefinition.HasValue())
        {
            logger.LogInformation("Object {ObjectName} with code: {Code} already exists. Nothing to create.", 
                nameof(KitSapDefinition), request.Code);
            return KitSapDefinitionErrors.ValidationCodeAlreadyExists(request.Code);
        }
        
        var kitSapDefinition = new KitSapDefinition(request.Code, request.Name);
        var kitSapDefinitionCreatedEvent = new KitSapDefinitionCreatedEvent(kitSapDefinition.Id, kitSapDefinition.Code, kitSapDefinition.Name);
        
        unitOfWork.StartStream<KitSapDefinitionCreatedEvent, KitSapDefinition>(kitSapDefinition.Id, kitSapDefinitionCreatedEvent);
        
        return kitSapDefinition;
    }
}