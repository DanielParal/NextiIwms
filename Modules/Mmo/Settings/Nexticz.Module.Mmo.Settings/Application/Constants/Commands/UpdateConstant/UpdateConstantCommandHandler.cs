using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Constants.Queries.GetConstantByKey;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Constants.Commands.UpdateConstant;

internal class UpdateConstantCommandHandler(
    ILogger<UpdateConstantCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender
    ) : IRequestHandler<UpdateConstantCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateConstantCommand request, CancellationToken cancellationToken)
    {
        var constant = await sender.Send(new GetConstantByKeyQuery(request.Key), cancellationToken);

        if (constant.IsError)
        {
            logger.LogWarning("Did not find object {ObjectName} with key: {Key}. Nothing to update", nameof(Constant), request.Key);
            return constant.Errors;
        }
        
        if (!ConstantValidator.IsValidValue(request.Value, constant.Value.Type))
        {
            logger.LogWarning("Object {ObjectName} with key: {Key} does not have valid value: {Value} for type: {Type}. Nothing to update.", 
                nameof(Constant), request.Key, request.Value, constant.Value.Type.ToString());
            return ConstantErrors.ValidationValueIsNotCorrectType(request.Value, constant.Value.Type);
        }
        
        var constantValueUpdatedEvent = new ConstantUpdatedEvent(request.Value, request.Description);
        unitOfWork.AppendEvent(constant.Value.Id, constantValueUpdatedEvent);
        return Result.Updated;
    }
}