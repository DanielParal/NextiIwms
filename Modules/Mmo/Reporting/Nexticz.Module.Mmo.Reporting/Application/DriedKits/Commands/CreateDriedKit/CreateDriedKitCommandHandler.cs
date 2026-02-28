using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Reporting.Application.DriedKits.Queries.GetDriedKitByIdFromDrying;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate.Events;

namespace Nexticz.Module.Mmo.Reporting.Application.DriedKits.Commands.CreateDriedKit;

internal class CreateDriedKitCommandHandler(
    ISender sender,
    ILogger<CreateDriedKitCommandHandler> logger,
    IReportingUnitOfWork unitOfWork) : IRequestHandler<CreateDriedKitCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(CreateDriedKitCommand request, CancellationToken cancellationToken)
    {
        var existingDriedKit = await sender.Send(new GetDriedKitByIdFromDryingQuery(request.KitIdFromDrying), cancellationToken);

        if (existingDriedKit.HasValue())
        {
            logger.LogWarning("Reporting - dried kit already exists. We cannot create new one. KitId: {KitId}, KitIdFromDrying: {KitIdFromDrying}", 
                existingDriedKit.Value.Id, existingDriedKit.Value.KitIdFromDrying);
            return DriedKitErrors.ValidationDriedKitAlreadyExist;
        }

        var driedKit = new DriedKit(request.KitIdFromDrying, request.BatchId, request.CompletedKitsCount, request.KitCode, request.LineCode,
            request.ExpectedDryingTime, request.DryingStarted, request.DryingEnded, request.Destination, request.TransferredToDryingSectionAt);
        
        var driedKitCreatedEvent = new DriedKitCreatedEvent(driedKit.Id, driedKit.KitIdFromDrying, driedKit.CompletedKitsCount, driedKit.BatchId,
            driedKit.KitCode, driedKit.LineCode,
            driedKit.ExpectedDryingTime, driedKit.DryingStarted, driedKit.DryingEnded, driedKit.Destination, driedKit.TransferredToDryingSectionAt);
        
        unitOfWork.StartStream<DriedKitCreatedEvent, DriedKit>(driedKit.Id, driedKitCreatedEvent);
        
        logger.LogInformation("Reporting - dried kit created. KitId: {KitId}, KitIdFromDrying: {KitIdFromDrying}.", driedKit.Id, request.KitIdFromDrying);
        return Result.Success;
    }
}