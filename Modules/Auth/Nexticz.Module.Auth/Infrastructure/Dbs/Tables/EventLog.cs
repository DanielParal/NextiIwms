namespace Nexticz.Module.Auth.Infrastructure.Dbs.Tables;

public class EventLog
{
    public Guid Id { get; set; }
    public Guid StreamId { get; set; }
    public string Data { get; set; }
    public string Type { get; set; }
    public string DotnetType { get; set; }
    public string CorrelationId { get; set; }
    public Guid? UserCreated { get; set; }
    public DateTimeOffset DateCreated { get; set; }
}