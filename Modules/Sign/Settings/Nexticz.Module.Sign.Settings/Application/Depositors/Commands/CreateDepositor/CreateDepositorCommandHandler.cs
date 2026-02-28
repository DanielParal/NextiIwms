using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorByCode;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Commands.CreateDepositor;

internal class CreateDepositorCommandHandler(
        ILogger<CreateDepositorCommandHandler> logger,
        ISettingsUnitOfWork unitOfWork,
        ISender sender
    ) : IRequestHandler<CreateDepositorCommand, ErrorOr<Depositor>>
{
    public async Task<ErrorOr<Depositor>> Handle(CreateDepositorCommand request, CancellationToken cancellationToken)
    {
        var existingDepositor = await sender.Send(new GetDepositorByCodeQuery(request.Code), cancellationToken);

        if (existingDepositor.HasValue())
        {
            logger.LogInformation("Sign - Settings - Object {ObjectName} with code: {Code} already exists. Nothing to create",
                nameof(Depositor), request.Code);
            return DepositorErrors.ValidationCodeAlreadyExists;
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
            
        var depositor = new Depositor(request.Code, request.Name, validationResult.Value.DepositorGroupCode, validationResult.Value.DeliveryTemplateCode, validationResult.Value.LoadingTemplateCode);
        var depositorCreatedEvent =
            new DepositorCreatedEvent(depositor.Id, depositor.Code, depositor.Name, depositor.DepositorGroupCode, depositor.DeliveryTemplateCode, depositor.LoadingTemplateCode);

        unitOfWork.StartStream<DepositorCreatedEvent, Depositor>(depositor.Id, depositorCreatedEvent);
            
        logger.LogInformation("Sign - Settings - Object {ObjectName} with code: {Code} created",
            nameof(Depositor), request.Code);
        return depositor;
    }
}