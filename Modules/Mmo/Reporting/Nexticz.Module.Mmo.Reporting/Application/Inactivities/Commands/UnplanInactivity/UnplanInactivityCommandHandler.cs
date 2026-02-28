using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemById;
using Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate.Events;

namespace Nexticz.Module.Mmo.Reporting.Application.Inactivities.Commands.UnplanInactivity;

internal class UnplanInactivityCommandHandler(
    ISender sender,
    IReportingUnitOfWork unitOfWork,
    ILogger<UnplanInactivityCommandHandler> logger,
    IClock clock,
    ICurrentUserProvider currentUserProvider) : IRequestHandler<UnplanInactivityCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UnplanInactivityCommand request, CancellationToken cancellationToken)
    {
        var lineItem = await sender.Send(new GetLineItemByIdQuery(request.Id), cancellationToken);
        
        if (lineItem.IsError)
        {
            logger.LogWarning("Reporting - line item does not exist. We cannot unplan inactivity. Id: {Id}.", request.Id);
            return lineItem.Errors;
        }
        
        var currentUserName = currentUserProvider.GetCurrentUser().UserName;
        var inactivityUnplannedEvent = new InactivityUnplannedEvent(request.Id, clock.UtcNowOffset, currentUserName);
        unitOfWork.AppendEvent(request.Id, inactivityUnplannedEvent);
        
        logger.LogInformation("Reporting - inactivity unplanned. Id: {Id}.", 
            request.Id);
        return Result.Success;
    }
}