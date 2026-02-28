using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Depositors.Queries.GetDepositorByCode;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Depositors.Commands.CreateDepositor;

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
            logger.LogInformation("Object {ObjectName} with code: {Code} already exists. Nothing to create.",
                nameof(Depositor), request.Name);
            return DepositorErrors.DepositorWithCodeAlreadyExists(request.Code);
        }
            
        var depositor = new Depositor(request.Code, request.Name, request.BarcodeTemplate);
        var depositorCreatedEvent =
            new DepositorCreatedEvent(depositor.Id, depositor.Code, depositor.Name, depositor.BarcodeTemplate);

        unitOfWork.StartStream<DepositorCreatedEvent, Depositor>(depositor.Id, depositorCreatedEvent);
            
        return depositor;
    }
}