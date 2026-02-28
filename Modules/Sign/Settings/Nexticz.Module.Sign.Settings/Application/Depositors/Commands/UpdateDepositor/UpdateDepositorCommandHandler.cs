using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorByCode;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Commands.UpdateDepositor;

internal class UpdateDepositorCommandHandler(
    ILogger<UpdateDepositorCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender) 
    : IRequestHandler<UpdateDepositorCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateDepositorCommand request, CancellationToken cancellationToken)
    {
        var depositor = await sender.Send(new GetDepositorByCodeQuery(request.Code), cancellationToken);

        if (depositor.IsError)
        {
            logger.LogWarning("Sign - Settings - Did not find object {ObjectName} with code: {Code}. Nothing to update", 
                nameof(Depositor), request.Code);
            return DepositorErrors.ValidationCodeDoesNotExist;
        }
        
        var validationResult = 
            await DepositorValidator.ValidateAsync(
                request.Code,
                request.DepositorGroupCode,
                request.DeliveryTemplateCode,
                request.LoadingTemplateCode,
                sender, logger, cancellationToken);  
        
        if (validationResult.IsError)
            return validationResult.Errors;
        
        var depositorUpdatedEvent = new DepositorUpdatedEvent(
            depositor.Value.Id, depositor.Value.Code, request.Name, validationResult.Value.DepositorGroupCode, validationResult.Value.DeliveryTemplateCode, validationResult.Value.LoadingTemplateCode);
        unitOfWork.AppendEvent(depositor.Value.Id, depositorUpdatedEvent);
        
        logger.LogInformation("Sign - Settings - Object {ObjectName} with code: {Code}, name: {Name} updated",
            nameof(Depositor), request.Code, request.Name);
        return Result.Updated;
    }
}