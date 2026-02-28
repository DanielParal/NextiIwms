using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.InactivityTypes.Queries;
using Nexticz.Lib.Shared.Time;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemById;
using Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate.Events;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.Inactivities.Commands.ChangeInactivityType;

internal class ChangeInactivityTypeCommandHandler(
    ISender sender,
    IReportingUnitOfWork unitOfWork,
    ILogger<ChangeInactivityTypeCommandHandler> logger,
    IClock clock,
    ICurrentUserProvider currentUserProvider) : IRequestHandler<ChangeInactivityTypeCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ChangeInactivityTypeCommand request, CancellationToken cancellationToken)
    {
        var lineItem = await sender.Send(new GetLineItemByIdQuery(request.Id), cancellationToken);
        
        if (lineItem.IsError)
        {
            logger.LogWarning("MMO - Reporting - line item does not exist. We cannot change inactivity type. Id: {Id}.", request.Id);
            return lineItem.Errors;
        }
        
        var inactivityReasonValuesAsync = await GetInactivityReasonValuesAsync(lineItem.Value, request.InactivityReasonId, request.Type, cancellationToken);
        if (inactivityReasonValuesAsync.IsError)
            return inactivityReasonValuesAsync.Errors;
        
        var currentUserName = currentUserProvider.GetCurrentUser().UserName;
        var inactivityTypeChangedEvent = 
            new InactivityTypeChangedEvent(request.Id, lineItem.Value.ShiftId, request.Type, request.InactivityReasonId, 
                inactivityReasonValuesAsync.Value.IsCommentNeededForReveiw, inactivityReasonValuesAsync.Value.AffectProductivity, clock.UtcNowOffset, currentUserName);
        unitOfWork.AppendEvent(request.Id, inactivityTypeChangedEvent);
        
        logger.LogInformation("MMO - Reporting - inactivity type changed. Id: {Id}, NewType: {NewType}.", 
            request.Id, request.Type);
        
        return Result.Success;
    }
    
    private async Task<ErrorOr<(bool AffectProductivity, bool IsCommentNeededForReveiw)>> GetInactivityReasonValuesAsync(LineItemView lineItemView, Guid? inactivityReasonId, InactivityType newType, CancellationToken cancellationToken)
    {
        if (inactivityReasonId is null)
            return (lineItemView.DoesTypeAffectsProductivityByDefault(), false);
        
        if (!lineItemView.IsTypeWhichMightAffectProductivity())
            return (false, false);
        
        var inactivityTypeFromSetting = await sender.Send(new GetInactivityTypeResponseByIdQuery(inactivityReasonId.Value), cancellationToken);
        if (inactivityTypeFromSetting.IsError)
        {
            logger.LogWarning("MMO - Reporting - inactivity type does not exist. We cannot change inactivity. Id: {Id}.", inactivityReasonId);
            return InactivityErrors.ValidationInactivityReasonDoesNotExistInSetting;
        }
        
        return (inactivityTypeFromSetting.Value.AffectProductivity, inactivityTypeFromSetting.Value.IsCommentNeededForReview);
    }
}