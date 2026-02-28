using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemById;
using Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate.Events;
using Nexticz.Module.Mmo.Reporting.Domain.KitAggregate.Events;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Commands.ChangeComment;

internal class ChangeCommentCommandHandler(
    ISender sender,
    ILogger<ChangeCommentCommandHandler> logger,
    IReportingUnitOfWork unitOfWork,
    IClock clock,
    ICurrentUserProvider currentUserProvider) 
    : IRequestHandler<ChangeCommentCommand, ErrorOr<LineItemView>>
{
    public async Task<ErrorOr<LineItemView>> Handle(ChangeCommentCommand request, CancellationToken cancellationToken)
    {
        var lineItem = await sender.Send(new GetLineItemByIdQuery(request.Id), cancellationToken);

        if (lineItem.IsError)
        {
            logger.LogInformation("MMO - Reporting - line item does not exist. We cannot add comment. Id: {Id}.", request.Id);
            return lineItem.Errors;
        }

        var currentUserName = currentUserProvider.GetCurrentUser().UserName;
        if (lineItem.Value.Type == LineItemType.Kit)
        {
            var kitCommentAddedEvent = new KitCommentChangedEvent(lineItem.Value.Id, request.Comment, clock.UtcNowOffset, currentUserName);
            unitOfWork.AppendEvent(lineItem.Value.Id, kitCommentAddedEvent);
            return lineItem.Value;
        }
        
        var inactivityCommentAddedEvent = new InactivityCommentAddedEvent(lineItem.Value.Id, lineItem.Value.ShiftId, request.Comment, clock.UtcNowOffset, currentUserName);
        unitOfWork.AppendEvent(lineItem.Value.Id, inactivityCommentAddedEvent);
        
        logger.LogInformation("MMO - Reporting - comment added. ItemId: {Id}, shiftId: {ShiftId}, comment: {Comment}", 
            request.Id, lineItem.Value.ShiftId, request.Comment);
        
        return lineItem.Value;
    }
}