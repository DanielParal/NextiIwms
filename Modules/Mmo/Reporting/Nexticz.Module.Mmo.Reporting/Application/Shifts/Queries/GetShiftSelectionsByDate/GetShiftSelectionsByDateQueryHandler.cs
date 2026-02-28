using MediatR;
using Nexticz.Module.Mmo.Reporting.Contracts.Shifts;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftsByDate;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftsByIds;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftStatus;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetUnapprovedShiftStatusViews;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftSelectionsByDate;

internal class GetShiftSelectionsByDateQueryHandler(
    ISender sender,
    IClock clock) : IRequestHandler<GetShiftSelectionsByDateQuery, ShiftSelectionResponse[]>
{
    public async Task<ShiftSelectionResponse[]> Handle(GetShiftSelectionsByDateQuery request, CancellationToken cancellationToken)
    {
        var todayShifts = await sender.Send(new GetShiftsByDateQuery(request.SelectedDate), cancellationToken);
        var unapprovedShiftStatuses = await sender.Send(new GetUnapprovedShiftStatusViewsQuery(), cancellationToken);
        
        
        var shiftSelectionResponses = new List<ShiftSelectionResponse>();

        foreach (var todayShift in todayShifts)
        {
            var shiftStatus = await sender.Send(new GetShiftStatusQuery(todayShift, unapprovedShiftStatuses), cancellationToken);
            var shiftSelectionResponse = new ShiftSelectionResponse(
                todayShift.Id,
                GetShiftName(todayShift), 
                clock.GetTenantDateOnly(todayShift.Schedule.End),
                (ShiftStatusContract)shiftStatus,
                ShiftSelectionCategoryContract.Today);
            
            shiftSelectionResponses.Add(shiftSelectionResponse);
        }
        
        var theRestUnapprovedShiftIds = unapprovedShiftStatuses.Select(x => x.ShiftId).Except(todayShifts.Select(x => x.Id)).ToArray();
        var theRestUnapprovedShifts = await sender.Send(new GetShiftsByIdsQuery(theRestUnapprovedShiftIds), cancellationToken);

        foreach (var theRestUnapprovedShift in theRestUnapprovedShifts.OrderByDescending(x => x.Schedule.End))
        {
            var shiftStatus = await sender.Send(new GetShiftStatusQuery(theRestUnapprovedShift, unapprovedShiftStatuses), cancellationToken);
            var shiftSelectionResponse = new ShiftSelectionResponse(
                theRestUnapprovedShift.Id,
                GetShiftName(theRestUnapprovedShift), 
                clock.GetTenantDateOnly(theRestUnapprovedShift.Schedule.End),
                (ShiftStatusContract)shiftStatus,
                ShiftSelectionCategoryContract.Other);
            
            shiftSelectionResponses.Add(shiftSelectionResponse);
        }
        
        return shiftSelectionResponses.ToArray();
    }

    private static string GetShiftName(Shift shift)
        => $"{shift.Name} {shift.Schedule.End:dd.MM.yyyy} ({shift.Schedule.Start:HH:mm} - {shift.Schedule.End:HH:mm})";
}