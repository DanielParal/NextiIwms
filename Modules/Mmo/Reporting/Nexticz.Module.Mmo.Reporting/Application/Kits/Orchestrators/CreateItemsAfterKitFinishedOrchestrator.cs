using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Constants.Queries;
using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Module.Mmo.Reporting.Application.Inactivities.Commands.ChangeInactivityTimeInterval;
using Nexticz.Module.Mmo.Reporting.Application.Inactivities.Commands.CreateInactivity;
using Nexticz.Module.Mmo.Reporting.Application.Inactivities.Commands.UnplanInactivity;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Application.Kits.Commands.CreateKit;
using Nexticz.Module.Mmo.Reporting.Application.Kits.Notifications;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.LineItemQueueBuilder;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.LineItemQueueBuilder.Models;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLastItemByLineCode;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetPlannedLineItemsByTimeRange;
using Nexticz.Module.Mmo.Reporting.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetNextToLastShift;
using Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.SpecialInformationEntity;
using Nexticz.Module.Mmo.Reporting.Domain.Views;
using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Kits.Orchestrators;

internal class CreateItemsAfterKitFinishedOrchestrator(
    ISender sender,
    IReportingUnitOfWork unitOfWork,
    ILogger<CreateItemsAfterKitFinishedOrchestrator> logger,
    IReportingNotificationCollector notificationCollector) : ICreateItemsAfterKitFinishedOrchestrator
{
    private readonly HashSet<Guid> _inactivityCreatedInShiftIds = [];
    
    public async Task<ErrorOr<Success>> OrchestrateAsync(Shift currentShift, KitWashingFinishedContract finishedKit,
        KitWashingFinishedContract? finishedSisterKit, CancellationToken cancellationToken)
    {
        unitOfWork.BeginTransaction();
        
        try
        {
            var result = finishedSisterKit is null
                ? await HandleSingleKitItemsAsync(currentShift, finishedKit, cancellationToken)
                : await HandleSisterKitsItemsAsync(currentShift, finishedKit, finishedSisterKit, cancellationToken);

            if (result.IsError)
            {
                logger.LogError("Reporting - rolling back CreateItemsAfterKitFinishedOrchestrator due to error. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}.",
                    result.FirstError.Code, result.FirstError.Description);
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return result.Errors;
            }

            await unitOfWork.CommitTransactionAsync(cancellationToken);

            if (_inactivityCreatedInShiftIds.Count > 0)
            {
                notificationCollector.AddNotification(new InactivityCreatedAfterKitFinishedNotification(_inactivityCreatedInShiftIds.ToArray()));
                await notificationCollector.PublishNotificationsAsync(cancellationToken);
            }
            
            return Result.Success;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Reporting - rolling back CreateItemsAfterKitFinishedOrchestrator due to error. Error Message: {ErrorMessage}.",
                exception.Message);
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task<ErrorOr<Success>> HandleSingleKitItemsAsync(
        Shift currentShift, KitWashingFinishedContract finishedKit,
        CancellationToken cancellationToken)
    {
        var adjustmentConstant = await sender.Send(new GetWashingMachineAdjustmentTimeConstantValueQuery(), cancellationToken);
        var lineData = await GetLastItemEndDateOnLineAndPlannedLineItemsAsync(finishedKit, currentShift.Schedule.Start, cancellationToken);
        
        var buildResponse = await LineItemQueueBuilder.BuildAsync(
            currentShift,
            finishedKit.WashingStarted, 
            finishedKit.WashingEnded, 
            finishedKit.OptimalKitDuration,
            lineData.LastItemOnLine, 
            adjustmentConstant, 
            lineData.PlannedLineItems,
            async () => await sender.Send(new GetNextToLastShiftQuery(), cancellationToken));
            
        var processResult = await ProcessLineItemQueueAsync(finishedKit, buildResponse, cancellationToken);
        return processResult;
    }
    
    private async Task<ErrorOr<Success>> HandleSisterKitsItemsAsync(
        Shift currentShift, KitWashingFinishedContract finishedKit, KitWashingFinishedContract finishedSisterKit,
        CancellationToken cancellationToken)
    {
        var adjustmentConstant = await sender.Send(new GetWashingMachineAdjustmentTimeConstantValueQuery(), cancellationToken);
        var line1Data = await GetLastItemEndDateOnLineAndPlannedLineItemsAsync(finishedKit, currentShift.Schedule.Start, cancellationToken);
        var line2Data = await GetLastItemEndDateOnLineAndPlannedLineItemsAsync(finishedSisterKit, currentShift.Schedule.Start, cancellationToken);
        var lastItemOnBothLines = SyncSisterLastItemEndDateOnLine(line1Data.LastItemOnLine, line2Data.LastItemOnLine);

        var sisterOptimalKitDuration = GetSisterOptimalKitDuration(finishedKit, finishedSisterKit);
        var funcToGetNextToLastShift = async () => await sender.Send(new GetNextToLastShiftQuery(), cancellationToken);
        var buildResponseLine1 = await LineItemQueueBuilder.BuildAsync(
            currentShift, finishedKit.WashingStarted, finishedKit.WashingEnded, sisterOptimalKitDuration, lastItemOnBothLines, adjustmentConstant, line1Data.PlannedLineItems, funcToGetNextToLastShift);
        var buildResponseLine2 = await LineItemQueueBuilder.BuildAsync(
            currentShift, finishedSisterKit.WashingStarted, finishedSisterKit.WashingEnded, sisterOptimalKitDuration, lastItemOnBothLines, adjustmentConstant, line2Data.PlannedLineItems, funcToGetNextToLastShift);
            
        var processResultLine1 = await ProcessLineItemQueueAsync(finishedKit, buildResponseLine1, cancellationToken);
        var processResultLine2 = await ProcessLineItemQueueAsync(finishedSisterKit, buildResponseLine2, cancellationToken);
        
        if (processResultLine1.IsError || processResultLine2.IsError)
            return processResultLine1.IsError ? processResultLine1 : processResultLine2;
        
        return Result.Success;
    }

    private static TimeSpan GetSisterOptimalKitDuration(KitWashingFinishedContract finishedKit,
        KitWashingFinishedContract finishedSisterKit)
    {
        return finishedKit.OptimalKitDuration > finishedSisterKit.OptimalKitDuration ? finishedKit.OptimalKitDuration : finishedSisterKit.OptimalKitDuration;   
    }

    private async Task<(LastItemPerLineView? LastItemOnLine, LineItemView[] PlannedLineItems)> GetLastItemEndDateOnLineAndPlannedLineItemsAsync(KitWashingFinishedContract finishedKit, DateTimeOffset shiftStartDate, CancellationToken cancellationToken)
    {
        var lastItemOnLine = await sender.Send(new GetLastItemByLineCodeQuery(finishedKit.LineCode), cancellationToken);
        var startDateForPlannedItems = lastItemOnLine?.LastFinishedDate ?? shiftStartDate;
        var plannedLineItems = 
            await sender.Send(new GetPlannedLineItemsByTimeRangeQuery(finishedKit.LineCode, startDateForPlannedItems, finishedKit.WashingEnded), cancellationToken);
        
        return (lastItemOnLine, plannedLineItems);
    }

    private async Task<ErrorOr<Success>> ProcessLineItemQueueAsync(KitWashingFinishedContract finishedKit,
        LineItemQueueResponse lineItemQueueResponse, CancellationToken cancellationToken)
    {
        // we need to shorten item which should be shortened
        if (lineItemQueueResponse.LineItemToShorten is not null)
        {
            var changeInterval = 
                await sender.Send(
                    new ChangeInactivityTimeIntervalCommand(
                        lineItemQueueResponse.LineItemToShorten.LineItemId, lineItemQueueResponse.LineItemToShorten.StartDate, lineItemQueueResponse.LineItemToShorten.EndDate), 
                    cancellationToken);
            
            if (changeInterval.IsError)
                return changeInterval.Errors;
        }
        
        // we need to update those which have ExistingId not null
        foreach (var lineItemToUnplan in lineItemQueueResponse.LineItemsToUnplan)
        {
            var unplanResult = await sender.Send(new UnplanInactivityCommand(lineItemToUnplan.ExistingId!.Value), cancellationToken);
            if (unplanResult.IsError)
                return unplanResult.Errors;
        }
        
        // we need to create those which have ExistingId null
        foreach (var lineItemToCreate in lineItemQueueResponse.LineItemsToCreate)
        {
            if (lineItemToCreate.Type == LineItemType.Kit)
            {
                var specialInformation = finishedKit.SpecialInformation is null ? null : 
                                         new SpecialInformation(finishedKit.SpecialInformation.Title, finishedKit.SpecialInformation.Description, 
                                             finishedKit.SpecialInformation.HasFile, finishedKit.SpecialInformation.WorkerName, finishedKit.SpecialInformation.DateConfirmed, finishedKit.SpecialInformation.Id);
                
                var createKitResult = await sender.Send(new CreateKitCommand(finishedKit.KitId, finishedKit.SisterKitId, lineItemToCreate.ShiftId, finishedKit.BatchId, finishedKit.SisterBatchId,
                    finishedKit.WashingMachineCode, finishedKit.LineCode, finishedKit.WashingMachineSpeed, (SpeedLevel)finishedKit.WashingMachineSpeedLevel, finishedKit.OrderId, finishedKit.TotalPlannedKitsCountInBatch,
                    finishedKit.KitCode, finishedKit.KitNumber, finishedKit.KitSapDefinitionCode, finishedKit.KitSapDefinitionName, finishedKit.SapBarcode, finishedKit.PackagingCode, finishedKit.OptimalPackagingSpeed,
                    (SpeedLevel)finishedKit.OptimalPackagingSpeedLevel, finishedKit.DefiningPackagingCode, finishedKit.WorkerName, finishedKit.GlobalKitsCount, lineItemToCreate.TotalDuration, finishedKit.OptimalKitDuration,
                    lineItemToCreate.StartDate, lineItemToCreate.EndDate, specialInformation), cancellationToken);
                
                if (createKitResult.IsError)
                    return createKitResult.Errors;
            }
            else
            {
                var createInactivityResult = await sender.Send(new CreateInactivityCommand(
                    lineItemToCreate.ShiftId, finishedKit.WashingMachineCode, finishedKit.LineCode, lineItemToCreate.StartDate, lineItemToCreate.EndDate, null, (InactivityType)lineItemToCreate.Type, false, null), cancellationToken);
                
                if (createInactivityResult.IsError)
                    return createInactivityResult.Errors;

                _inactivityCreatedInShiftIds.Add(lineItemToCreate.ShiftId);
            }
        }
        
        return Result.Success;
    }

    private static LastItemPerLineView? SyncSisterLastItemEndDateOnLine(LastItemPerLineView? lastItemOnLine1,
        LastItemPerLineView? lastItemOnLine2)
    {
        if (lastItemOnLine1 is null && lastItemOnLine2 is null)
            return null;
        
        if (lastItemOnLine1 is null)
            return lastItemOnLine2;
        
        if (lastItemOnLine2 is null)
            return lastItemOnLine1;
        
        return lastItemOnLine1.LastFinishedDate > lastItemOnLine2.LastFinishedDate ? lastItemOnLine1 : lastItemOnLine2;
    }
}
        