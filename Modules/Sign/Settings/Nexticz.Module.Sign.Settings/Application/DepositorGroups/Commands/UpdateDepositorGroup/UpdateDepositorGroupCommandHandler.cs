using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.DepositorGroups.Queries.GetDepositorGroupByCode;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.Commands.UpdateDepositorGroup;

internal class UpdateDepositorGroupCommandHandler(
    ILogger<UpdateDepositorGroupCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender) 
    : IRequestHandler<UpdateDepositorGroupCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateDepositorGroupCommand request, CancellationToken cancellationToken)
    {
        var depositorGroup = await sender.Send(new GetDepositorGroupByCodeQuery(request.Code), cancellationToken);

        if (depositorGroup.IsError)
        {
            logger.LogWarning("Sign - Did not find object {ObjectName} with code: {Code}. Nothing to update", 
                nameof(DepositorGroup), request.Code);
            return DepositorGroupErrors.ValidationCodeDoesNotExist;
        }
        
        var depositorGroupUpdatedEvent = new DepositorGroupUpdatedEvent(depositorGroup.Value.Id, request.Code, request.Name);
        unitOfWork.AppendEvent(depositorGroup.Value.Id, depositorGroupUpdatedEvent);
        
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code}, name: {Name} updated.",
            nameof(DepositorGroup), request.Code, request.Name);
        return Result.Updated;
    }
}