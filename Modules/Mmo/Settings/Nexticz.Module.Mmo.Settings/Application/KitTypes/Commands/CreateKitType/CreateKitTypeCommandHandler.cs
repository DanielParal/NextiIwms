using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.KitTypes.Queries.GetKitTypeByCode;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.KitTypes.Commands.CreateKitType;

internal class CreateKitTypeCommandHandler(
    ILogger<CreateKitTypeCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender)
    : IRequestHandler<CreateKitTypeCommand, ErrorOr<KitType>>
{
    public async Task<ErrorOr<KitType>> Handle(CreateKitTypeCommand request, CancellationToken cancellationToken)
    {
        var existingKitType = await sender.Send(new GetKitTypeByCodeQuery(request.Code), cancellationToken);
        if (existingKitType.HasValue())
        {
            logger.LogInformation("Object {ObjectName} with code: {Code} already exists. Nothing to create.", nameof(KitType), request.Code);
            return KitTypeErrors.ValidationCodeAlreadyExists(request.Code);
        }
        
        var kitType = new KitType(request.Code, request.Name);
        var kitTypeCreated = new KitTypeCreatedEvent(kitType.Id, kitType.Code, kitType.Name);
        
        unitOfWork.StartStream<KitTypeCreatedEvent, KitType>(kitType.Id, kitTypeCreated);
        
        return kitType;
    }
}