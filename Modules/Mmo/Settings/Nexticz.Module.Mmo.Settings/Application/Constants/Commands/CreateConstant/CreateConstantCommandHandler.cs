using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Constants.Queries.GetConstantByKey;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Constants.Commands.CreateConstant;

internal class CreateConstantCommandHandler(
    ILogger<CreateConstantCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender)
    : IRequestHandler<CreateConstantCommand, ErrorOr<Constant>>
{
    public async Task<ErrorOr<Constant>> Handle(CreateConstantCommand request, CancellationToken cancellationToken)
    {
        var existingConstant = await sender.Send(new GetConstantByKeyQuery(request.Key), cancellationToken);
        if (existingConstant.HasValue())
        {
            logger.LogInformation("Object {ObjectName} with key: {Key} already exists. Nothing to create.", nameof(Constant), request.Key);
            return ConstantErrors.ValidationKeyAlreadyExists(request.Key);
        }

        if (!ConstantValidator.IsValidValue(request.Value, request.Type))
        {
            logger.LogInformation("Object {ObjectName} with key: {Key} does not have valid value: {Value} for type: {Type}. Nothing to create.", 
                nameof(Constant), request.Key, request.Value, request.Type.ToString());
            return ConstantErrors.ValidationValueIsNotCorrectType(request.Value, request.Type);
        }
        
        var constant = new Constant(request.Key, request.Value, request.Type, request.Description);
        var constantCreatedEvent = new ConstantCreatedEvent(
            constant.Id, constant.Key, constant.Value, constant.Type, constant.Description);
        
        unitOfWork
            .StartStream<ConstantCreatedEvent, Constant>(
                constant.Id, constantCreatedEvent);
        
        return constant;
    }
}