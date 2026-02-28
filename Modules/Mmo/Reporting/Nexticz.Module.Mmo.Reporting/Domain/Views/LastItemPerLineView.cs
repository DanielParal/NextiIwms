namespace Nexticz.Module.Mmo.Reporting.Domain.Views;

public class LastItemPerLineView
{
    public string Id { get; set; }
    public Guid ItemId { get; set; }
    public Guid ShiftId { get; set; }
    public DateTimeOffset LastFinishedDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}