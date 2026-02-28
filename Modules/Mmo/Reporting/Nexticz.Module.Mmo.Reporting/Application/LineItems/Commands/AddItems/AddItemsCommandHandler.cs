using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Mmo.Reporting.Application.Inactivities.Commands.CreateInactivity;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Notifications;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLastItemByLineCode;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemsByTimeRange;
using Nexticz.Module.Mmo.Reporting.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftById;
using Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Commands.AddItems;

internal class AddItemsCommandHandler(
    ISender sender,
    ILogger<AddItemsCommandHandler> logger,
    IReportingUnitOfWork unitOfWork,
    ICurrentUserProvider currentUserProvider,
    IReportingNotificationCollector notificationCollector) : IRequestHandler<AddItemsCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(AddItemsCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateAsync(request, cancellationToken);
        if (validationResult.IsError)
            return validationResult.Errors;

        unitOfWork.BeginTransaction();
        
        InactivityType? addedType = null;
        var userName = currentUserProvider.GetCurrentUser().UserName;
        foreach (var washingMachineAndLineCode in validationResult.Value.LineInfos)
        {
            var result = await sender.Send(
                new CreateInactivityCommand(
                    validationResult.Value.ShiftId,
                    washingMachineAndLineCode.WashingMachineCode,
                    washingMachineAndLineCode.LineCode,
                    validationResult.Value.StartDate,
                    validationResult.Value.EndDate,
                    null,
                    validationResult.Value.Type,
                    washingMachineAndLineCode.IsPlanned,
                    userName),
                cancellationToken);

            if (!result.IsError) 
                continue;
            
            addedType = validationResult.Value.Type;
            logger.LogWarning("MMO - Reporting - cannot add items because one is failing. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}, LineCode: {LineCode}.",
                result.FirstError.Code, result.FirstError.Description, washingMachineAndLineCode.LineCode);
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            return result.Errors;
        }
        
        await unitOfWork.CommitTransactionAsync(cancellationToken);
        
        logger.LogInformation("MMO - Reporting - items added to the shift. ShiftId: {ShiftId}, Type: {Type}",
            request.ShiftId, request.Type);
        
        if (addedType is not null)
            notificationCollector.AddNotification(new ItemsAddedNotification(request.ShiftId, addedType.Value));
        
        return Result.Success;
    }
    
    private async Task<ErrorOr<ValidationResult>> ValidateAsync(AddItemsCommand request, CancellationToken cancellationToken)
    {
        if (request.StartDate >= request.EndDate)
        {
            logger.LogWarning("MMO - Reporting - shift does not exist. We cannot add item. ShiftId: {ShiftId}.", 
                request.ShiftId);
            return LineItemErrors.ValidationEndDateHasToBeGraterThanStartDate;
        }
        
        if (!Enum.TryParse<InactivityType>(request.Type.ToString(), out var inactivityType))
        {
            logger.LogWarning("MMO - Reporting - invalid line item type. " +
                "ShiftId: {ShiftId}, Type: {Type}, StartDate: {StartDate}, EndDate: {EndDate}.",
                request.ShiftId, request.Type, request.StartDate, request.EndDate);
            return LineItemErrors.ValidationAddOnlyBreakOrShutdown;
        }

        if (inactivityType is not (InactivityType.Break or InactivityType.Shutdown))
        {
            logger.LogWarning("MMO - Reporting - we can add only break or shutdown. " +
                              "ShiftId: {ShiftId}, Type: {Type}, StartDate: {StartDate}, EndDate: {EndDate}.", 
                request.ShiftId, request.Type, request.StartDate, request.EndDate);
            return LineItemErrors.ValidationAddOnlyBreakOrShutdown;
        }

        if (request.LineCodes.Length == 0)
        {
            logger.LogWarning("MMO - Reporting - no line codes provided. " +
                              "ShiftId: {ShiftId}, Type: {Type}, StartDate: {StartDate}, EndDate: {EndDate}.", 
                request.ShiftId, request.Type, request.StartDate, request.EndDate);
            return LineItemErrors.ValidationNoLineCodesProvided;
        }
        
        var shift = await sender.Send(new GetShiftByIdQuery(request.ShiftId), cancellationToken);
        
        if (shift.IsError)
        {
            logger.LogWarning("MMO - Reporting - shift does not exist. We cannot add item. ShiftId: {ShiftId}.", 
                request.ShiftId);
            return shift.Errors;
        }

        if (shift.Value.Schedule.Start > request.StartDate || shift.Value.Schedule.End < request.EndDate)
        {
            logger.LogWarning("MMO - Reporting - shift does not exist. We cannot add item. ShiftId: {ShiftId}.", 
                request.ShiftId);
            return LineItemErrors.ValidationItemTimeRangeMustBeWithinShiftTimeRange;
        }
        
        var lineItemsInTimeRange = await sender.Send(new GetLineItemsByTimeRangeQuery(request.StartDate, request.EndDate), cancellationToken);
        var lineItemsInTimeRangeWithLineCodes = lineItemsInTimeRange.Where(x => request.LineCodes.Contains(x.LineCode)).ToArray();
        if (lineItemsInTimeRangeWithLineCodes.Length > 0)
        {
            logger.LogWarning("MMO - Reporting - there are other items in the time range. We cannot add item. " +
                              "ShiftId: {ShiftId}, StartDate: {StartDate}, EndDate: {EndDate}.", 
                request.ShiftId, request.StartDate, request.EndDate);
            return LineItemErrors.ValidationOtherItemsInTimeRange;
        }

        var washingMachinesFromSettings = await sender.Send(new GetWashingMachineResponsesQuery(), cancellationToken);
        
        var lineInfos = new List<(string WashingMachineCode, string LineCode, bool IsPlanned)>();
        var uniqueLineCodes = request.LineCodes.Select(x => x.Trim().ToUpperInvariant()).Distinct();
        foreach (var lineCode in uniqueLineCodes)
        {
            var machineResponse = washingMachinesFromSettings
                .FirstOrDefault(
                    w => w.WashingMachineLines.Any(
                        l => l.Code.Equals(lineCode, StringComparison.InvariantCultureIgnoreCase)));

            var lastItemInQueue = await sender.Send(new GetLastItemByLineCodeQuery(lineCode), cancellationToken);

            var isPlanned = lastItemInQueue is null || lastItemInQueue.LastFinishedDate < request.StartDate;
            
            if (machineResponse is not null)
                lineInfos.Add((machineResponse.Code, lineCode, isPlanned));
        }

        if (lineInfos.Count == 0)
        {
            logger.LogWarning("MMO - Reporting - no valid line codes provided." +
                              "ShiftId: {ShiftId}, Type: {Type}, StartDate: {StartDate}, EndDate: {EndDate}.", 
                request.ShiftId, request.Type, request.StartDate, request.EndDate);
            return LineItemErrors.ValidationNoValidLineCodesProvided;
        }
        
        return new ValidationResult(
            request.ShiftId, request.StartDate, request.EndDate, inactivityType, lineInfos);
    }
    
    private record ValidationResult(
        Guid ShiftId, 
        DateTimeOffset StartDate, 
        DateTimeOffset EndDate, 
        InactivityType Type,
        List<(string WashingMachineCode, string LineCode, bool IsPlanned)> LineInfos);
}