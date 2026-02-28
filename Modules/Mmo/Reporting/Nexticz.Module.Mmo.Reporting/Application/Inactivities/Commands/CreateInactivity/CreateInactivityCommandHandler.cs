using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.InactivityTypes.Queries;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate.Events;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.Inactivities.Commands.CreateInactivity;

internal class CreateInactivityCommandHandler(
    ISender sender,
    ILogger<CreateInactivityCommandHandler> logger,
    IReportingUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<CreateInactivityCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(CreateInactivityCommand request, CancellationToken cancellationToken)
    {
        var inactivityReasonValuesAsync = await GetAffectProductivityValueAsync(request.InactivityReasonId, request.Type, cancellationToken);
        
        if (inactivityReasonValuesAsync.IsError)
            return inactivityReasonValuesAsync.Errors;
        
        var inactivity = new Inactivity(
            request.ShiftId,
            request.WashingMachineCode,
            request.LineCode,
            request.StartDate,
            request.EndDate,
            request.InactivityReasonId,
            inactivityReasonValuesAsync.Value.IsCommentNeededForReveiw,
            inactivityReasonValuesAsync.Value.AffectProductivity,
            request.Type,
            request.IsPlanned,
            clock.UtcNowOffset,
            request.DeclaredBy);
        
        var inactivityCreatedEvent = new InactivityCreatedEvent(
            inactivity.Id, inactivity.ShiftId, inactivity.WashingMachineCode, inactivity.LineCode,
            inactivity.StartDate, inactivity.EndDate, inactivity.InactivityReasonId, 
            inactivity.IsCommentNeededForReview, inactivity.AffectProductivity, 
            inactivity.Type, inactivity.IsPlanned, inactivity.CreatedAt, inactivity.DeclaredBy);
        
        unitOfWork.AppendEvent(inactivity.Id, inactivityCreatedEvent);
        
        logger.LogInformation("Reporting - Inactivity created. Id: {Id}, ShiftId: {ShiftId}, LineCode: {LineCode}, Type: {Type}.",
            inactivity.Id, inactivity.WashingMachineCode, inactivity.LineCode, inactivity.Type);
        
        return Result.Success;
    }

    private async Task<ErrorOr<(bool AffectProductivity, bool IsCommentNeededForReveiw)>> GetAffectProductivityValueAsync(Guid? inactivityReasonId, InactivityType newType, CancellationToken cancellationToken)
    {
        if (inactivityReasonId is null)
            return (LineItemView.DoesTypeAffectsProductivityByDefault((LineItemType)newType), false);
        
        if (!LineItemView.IsTypeWhichMightAffectProductivity((LineItemType)newType))
            return (false, false);
        
        var inactivityTypeFromSetting = await sender.Send(new GetInactivityTypeResponseByIdQuery(inactivityReasonId.Value), cancellationToken);
        if (inactivityTypeFromSetting.IsError)
        {
            logger.LogWarning("Reporting - inactivity type does not exist. We cannot create inactivity. Id: {Id}.", inactivityReasonId);
            return InactivityErrors.ValidationInactivityReasonDoesNotExistInSetting;
        }
        
        return (inactivityTypeFromSetting.Value.AffectProductivity, inactivityTypeFromSetting.Value.IsCommentNeededForReview);
    }
}