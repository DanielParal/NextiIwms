using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel.Efficiencies;
using Nexticz.Lib.Shared.Time;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemById;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemsByKitId;
using Nexticz.Module.Mmo.Reporting.Domain.KitAggregate.Events;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.Kits.Commands.ChangeKitTimeInterval;

internal class ChangeKitTimeIntervalCommandHandler(
    ISender sender,
    IReportingUnitOfWork unitOfWork,
    ILogger<ChangeKitTimeIntervalCommandHandler> logger,
    IClock clock,
    ICurrentUserProvider currentUserProvider) : IRequestHandler<ChangeKitTimeIntervalCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ChangeKitTimeIntervalCommand request, CancellationToken cancellationToken)
    {
        var lineItem = await sender.Send(new GetLineItemByIdQuery(request.Id), cancellationToken);
        
        if (lineItem.IsError)
        {
            logger.LogWarning("Reporting - line item does not exist. We cannot change kit time interval. Id: {Id}.", request.Id);
            return lineItem.Errors;
        }

        if (request.StartDate > request.EndDate)
        {
            logger.LogWarning("Reporting - line start date must be before end date. We cannot change kit time interval. Id: {Id}, StartDate: {StartDate}, EndDate: {EndDate}.", 
                request.Id, request.StartDate, request.EndDate);
            return KitErrors.ValidationStartDateBeforeEndDate;
        }

        if (lineItem.Value.Type != LineItemType.Kit)
        {
            logger.LogWarning("Reporting - line item is not type of kit. Id: {Id}, Type: {Type}, StartDate: {StartDate}, EndDate: {EndDate}.", 
                request.Id, lineItem.Value.Type, request.StartDate, request.EndDate);
            return KitErrors.ValidationLineItemIsNotKitType;
        }
        
        var lineItemParts = await sender.Send(new GetLineItemsByKitIdQuery(lineItem.Value.KitId!.Value), cancellationToken);
        var lineItemPartsExceptChangedOne = lineItemParts.Where(x => x.Id != lineItem.Value.Id).ToArray();
        
        var totalRealTime = GetTotalRealTime(request.StartDate, request.EndDate, lineItemPartsExceptChangedOne);

        var userName = currentUserProvider.GetCurrentUser().UserName;
        foreach (var lineItemPart in lineItemPartsExceptChangedOne)
        {
            var kitPartTimeIntervalChangedEvent = new KitTimeIntervalChangedEvent(
                lineItemPart.Id, lineItemPart.StartDate, lineItemPart.EndDate, totalRealTime, 
                EfficiencyCalculator.Calculate(totalRealTime, lineItemPart.OptimalKitDuration!.Value),
                clock.UtcNowOffset, userName);
            
            unitOfWork.AppendEvent(lineItemPart.Id, kitPartTimeIntervalChangedEvent);
        }
        
        var kitTimeIntervalChangedEvent = new KitTimeIntervalChangedEvent(
            request.Id, request.StartDate, request.EndDate, totalRealTime, 
            EfficiencyCalculator.Calculate(totalRealTime, lineItem.Value.OptimalKitDuration!.Value),
            clock.UtcNowOffset, userName);
        
        unitOfWork.AppendEvent(request.Id, kitTimeIntervalChangedEvent);
        
        logger.LogInformation("Reporting - kit time interval changed. Id: {Id}, StartDate: {StartDate}, EndDate: {EndDate}.", 
            request.Id, request.StartDate, request.EndDate);
        return Result.Success;
    }

    private static TimeSpan GetTotalRealTime(DateTimeOffset changedItemStartDate, DateTimeOffset changedItemEndDate, LineItemView[] allKitParts)
    {
        var changedLineItemDuration = changedItemEndDate - changedItemStartDate;
        var allPartsDuration = allKitParts
            .Select(x => x.EndDate - x.StartDate)
            .Aggregate(TimeSpan.Zero, (total, next) => total + next);
        
        return changedLineItemDuration + allPartsDuration;
    }
}