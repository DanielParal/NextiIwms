namespace Nexticz.Module.Mmo.Reporting.Domain.Views;

public class CurrentShiftView
{
    public Guid Id { get; set; }
    public Guid ShiftId { get; set; }
    public string Name { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}