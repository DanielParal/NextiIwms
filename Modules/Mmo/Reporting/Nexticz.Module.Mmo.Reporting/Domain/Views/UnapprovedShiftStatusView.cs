using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Domain.Views;

public class UnapprovedShiftStatusView
{
    public Guid Id { get; set; }
    public Guid ShiftId { get; set; }
    public ShiftStatus Status { get; set; }
}