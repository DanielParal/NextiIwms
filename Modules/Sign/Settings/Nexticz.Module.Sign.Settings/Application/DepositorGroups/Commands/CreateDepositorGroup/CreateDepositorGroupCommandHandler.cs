using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.DepositorGroups.Queries.GetDepositorGroupByCode;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.Commands.CreateDepositorGroup;

internal class CreateDepositorGroupCommandHandler(
        ILogger<CreateDepositorGroupCommandHandler> logger,
        ISettingsUnitOfWork unitOfWork,
        ISender sender
    ) : IRequestHandler<CreateDepositorGroupCommand, ErrorOr<DepositorGroup>>
{
    public async Task<ErrorOr<DepositorGroup>> Handle(CreateDepositorGroupCommand request, CancellationToken cancellationToken)
    {
        var existingDepositorGroup = await sender.Send(new GetDepositorGroupByCodeQuery(request.Code), cancellationToken);

        if (existingDepositorGroup.HasValue())
        {
            logger.LogInformation("Sign - Object {ObjectName} with code: {Code} already exists. Nothing to create.",
                nameof(DepositorGroup), request.Code);
            return DepositorGroupErrors.ValidationCodeAlreadyExists;
        }
            
        var depositorGroup = new DepositorGroup(request.Code, request.Name);
        var depositorGroupCreatedEvent =
            new DepositorGroupCreatedEvent(depositorGroup.Id, depositorGroup.Code, depositorGroup.Name);

        unitOfWork.StartStream<DepositorGroupCreatedEvent, DepositorGroup>(depositorGroup.Id, depositorGroupCreatedEvent);
            
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code} created.",
            nameof(DepositorGroup), request.Code);
        return depositorGroup;
    }
}