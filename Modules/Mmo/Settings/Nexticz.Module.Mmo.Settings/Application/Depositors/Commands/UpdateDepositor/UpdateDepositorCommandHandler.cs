using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Depositors.Queries.GetDepositorByCode;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Depositors.Commands.UpdateDepositor;

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
            logger.LogWarning("Did not find object {ObjectName} with code: {Code}. Nothing to update", nameof(Domain.DepositorEntity.Depositor), request.Code);
            return depositor.Errors;
        }
        
        var depositorUpdatedEvent = new DepositorUpdatedEvent(depositor.Value.Id, request.Name, request.BarcodeTemplate);
        unitOfWork.AppendEvent(depositor.Value.Id, depositorUpdatedEvent);
        return Result.Updated;
    }
}