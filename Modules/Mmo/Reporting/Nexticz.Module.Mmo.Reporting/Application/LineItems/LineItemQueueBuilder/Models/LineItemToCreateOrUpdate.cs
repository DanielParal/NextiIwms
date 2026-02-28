using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.LineItemQueueBuilder.Models;


internal class LineItemToCreateOrUpdate
    {
    public Guid ShiftId { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    public TimeSpan TotalDuration { get; private set; }
    public LineItemType Type { get; private set; }
    public Guid? ExistingId { get; private set; }

    public LineItemToCreateOrUpdate(
        Guid shiftId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        TimeSpan totalDuration,
        LineItemType type,
        Guid? existingId)
    {
        ShiftId = shiftId;
        StartDate = startDate;
        EndDate = endDate;
        TotalDuration = totalDuration;
        Type = type;
        ExistingId = existingId;
    }
    
    public void UpdateTotalDuration(TimeSpan totalDuration)
    {
        TotalDuration = totalDuration;
    }
}