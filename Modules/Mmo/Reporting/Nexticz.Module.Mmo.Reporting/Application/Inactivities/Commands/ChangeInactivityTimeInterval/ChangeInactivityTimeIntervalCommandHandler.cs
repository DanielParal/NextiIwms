using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemById;
using Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate.Events;

namespace Nexticz.Module.Mmo.Reporting.Application.Inactivities.Commands.ChangeInactivityTimeInterval;

internal class ChangeInactivityTimeIntervalCommandHandler(
    ISender sender,
    IReportingUnitOfWork unitOfWork,
    ILogger<ChangeInactivityTimeIntervalCommandHandler> logger,
    IClock clock,
    ICurrentUserProvider currentUserProvider) : IRequestHandler<ChangeInactivityTimeIntervalCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ChangeInactivityTimeIntervalCommand request, CancellationToken cancellationToken)
    {
        var lineItem = await sender.Send(new GetLineItemByIdQuery(request.Id), cancellationToken);
        
        if (lineItem.IsError)
        {
            logger.LogWarning("MMO - Reporting - line item does not exist. We cannot change inactivity time interval. Id: {Id}.", request.Id);
            return lineItem.Errors;
        }

        if (request.StartDate > request.EndDate)
        {
            logger.LogWarning("MMO - Reporting - line start date must be before end date. We cannot change inactivity time interval. Id: {Id}, StartDate: {StartDate}, EndDate: {EndDate}.", 
                request.Id, request.StartDate, request.EndDate);
            return InactivityErrors.ValidationStartDateBeforeEndDate;
        }

        var currentUserName = currentUserProvider.GetCurrentUser().UserName;
        var inactivityTimeIntervalChangedEvent = new InactivityTimeIntervalChangedEvent(request.Id, request.StartDate, request.EndDate, clock.UtcNowOffset, currentUserName);
        unitOfWork.AppendEvent(request.Id, inactivityTimeIntervalChangedEvent);
        
        logger.LogInformation("MMO - Reporting - inactivity time interval changed. Id: {Id}, StartDate: {StartDate}, EndDate: {EndDate}.", 
            request.Id, request.StartDate, request.EndDate);
        return Result.Success;
    }
}