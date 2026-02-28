using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Reporting.Contracts.LineItems;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Mmo.Reporting.Application.Inactivities.Commands.ChangeInactivityTimeInterval;
using Nexticz.Module.Mmo.Reporting.Application.Inactivities.Commands.ChangeInactivityType;
using Nexticz.Module.Mmo.Reporting.Application.Inactivities.Commands.CreateInactivity;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Application.Kits.Commands.ChangeKitTimeInterval;
using Nexticz.Module.Mmo.Reporting.Application.Kits.Commands.DeleteKit;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Notifications;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemById;
using Nexticz.Module.Mmo.Reporting.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Orchestrators.ChangeItem;

internal class ChangeItemOrchestrator(
    ISender sender,
    ILogger<ChangeItemOrchestrator> logger,
    IReportingUnitOfWork unitOfWork,
    IReportingNotificationCollector notificationCollector,
    ICurrentUserProvider currentUserProvider) : IChangeItemOrchestrator
{
    public async Task<ErrorOr<Success>> ChangeItemAsync(
        Guid lineItemId, DateTimeOffset cutTime, ChangeLineItemTypeContract newType,
        Guid? inactivityReasonId, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateAsync(lineItemId, cutTime, newType, inactivityReasonId, cancellationToken);
        if (validationResult.IsError)
            return validationResult.Errors;

        unitOfWork.BeginTransaction();
        
        var result = await ChangeItemAsync(validationResult.Value, cancellationToken);
        if (result.IsError)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            return result.Errors;
        }
        
        await unitOfWork.CommitTransactionAsync(cancellationToken);
        
        notificationCollector.AddNotification(new LineItemsChangedNotification(validationResult.Value.LineItem.ShiftId));
        await notificationCollector.PublishNotificationsAsync(cancellationToken);
        
        return Result.Success;
    }

    private async Task<ErrorOr<Success>> ChangeItemAsync(ValidationResult validationResult, CancellationToken cancellationToken)
    {
        var userName = currentUserProvider.GetCurrentUser().UserName;
        if (validationResult.LineItem.StartDate == validationResult.CutTime)
        {
            return await ChangeCompleteItemTypeAsync(validationResult, userName, cancellationToken);
        }
        
        return await SplitItemAsync(validationResult, userName, cancellationToken);
    }

    private async Task<ErrorOr<Success>> SplitItemAsync(ValidationResult validationResult, string declaredBy,
        CancellationToken cancellationToken)
    {
        var resultChangeTimeInterval = validationResult.LineItem.Type == LineItemType.Kit
            ? await sender.Send(
                new ChangeKitTimeIntervalCommand(validationResult.LineItem.Id, validationResult.LineItem.StartDate,
                    validationResult.CutTime), cancellationToken)
            : await sender.Send(
                new ChangeInactivityTimeIntervalCommand(validationResult.LineItem.Id,
                    validationResult.LineItem.StartDate, validationResult.CutTime), cancellationToken);
        
        if (resultChangeTimeInterval.IsError)
            return resultChangeTimeInterval.Errors;
        
        var resultCreateInactivity = await sender.Send(
            new CreateInactivityCommand(
                validationResult.LineItem.ShiftId, 
                validationResult.LineItem.WashingMachineCode, 
                validationResult.LineItem.LineCode, 
                validationResult.CutTime, 
                validationResult.LineItem.EndDate, 
                validationResult.NewInactivityReasonId,
                validationResult.NewType,
                validationResult.LineItem.IsPlanned,
                declaredBy), 
            cancellationToken);
            
        if (resultCreateInactivity.IsError)
            return resultCreateInactivity.Errors;
            
        return Result.Success;
    }

    private async Task<ErrorOr<Success>> ChangeCompleteItemTypeAsync(ValidationResult validationResult, string declaredBy,
        CancellationToken cancellationToken)
    {
        if (validationResult.LineItem.Type == LineItemType.Kit)
        {
            var resultDeleteKit = await sender.Send(new DeleteKitCommand(validationResult.LineItem.Id), cancellationToken);
            if (resultDeleteKit.IsError)
                return resultDeleteKit.Errors;
            
            var resultCreateInactivity = await sender.Send(
                new CreateInactivityCommand(
                    validationResult.LineItem.ShiftId, 
                    validationResult.LineItem.WashingMachineCode, 
                    validationResult.LineItem.LineCode, 
                    validationResult.LineItem.StartDate, 
                    validationResult.LineItem.EndDate, 
                    validationResult.NewInactivityReasonId,
                    validationResult.NewType,
                    validationResult.LineItem.IsPlanned,
                    declaredBy), 
                cancellationToken);
            
            if (resultCreateInactivity.IsError)
                return resultCreateInactivity.Errors;
            
            return Result.Success;
        }
        
        return await sender.Send(new ChangeInactivityTypeCommand(validationResult.LineItem.Id, validationResult.NewType, validationResult.NewInactivityReasonId), cancellationToken);
    }
    
    private async Task<ErrorOr<ValidationResult>> ValidateAsync(Guid lineItemId, DateTimeOffset cutTime, ChangeLineItemTypeContract newType, 
        Guid? newInactivityReasonId, CancellationToken cancellationToken)
    {
        var lineItem = await sender.Send(new GetLineItemByIdQuery(lineItemId), cancellationToken);
        
        if (lineItem.IsError)
        {
            logger.LogWarning("Reporting - line item does not exist. We cannot change item. Id: {Id}.", lineItemId);
            return lineItem.Errors;
        }
        
        if (!Enum.TryParse<InactivityType>(newType.ToString(), out var inactivityType))
        {
            logger.LogWarning("Reporting - failed to parse line item type contract from {NewType}. LineItemId: {LineItemId}.", newType, lineItemId);
            return LineItemErrors.ValidationLineItemTypeIsNotValid;
        }

        if (lineItem.Value.EndDate < cutTime)
        {
             logger.LogWarning("Reporting - cut time is not within item interval. LineItemId: {LineItemId}, CutTime: {CutTime}.",
                lineItemId, cutTime);
            return LineItemErrors.ValidationCutTimeNotWithinItemInterval;
        }
        
        if (lineItem.Value.StartDate > cutTime)
            cutTime = lineItem.Value.StartDate;
        
        return new ValidationResult(lineItem.Value, inactivityType, newInactivityReasonId, cutTime);
    }
    
    private record ValidationResult(LineItemView LineItem, InactivityType NewType, Guid? NewInactivityReasonId, DateTimeOffset CutTime);
}