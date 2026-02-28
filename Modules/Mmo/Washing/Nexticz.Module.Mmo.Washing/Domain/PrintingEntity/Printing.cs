using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;

namespace Nexticz.Module.Mmo.Washing.Domain.PrintingEntity;

public class Printing : Entity
{
    public Guid BatchId { get; private set; }
    public Guid KitId { get; private set; }
    public DateTimeOffset DatePrinted { get; private set; }
    public PrintingStatus Status { get; private set; }
    public PrintingType Type { get; private set; }
    public string? FailureReason { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Printing() {}

    public Printing(
        Guid batchId,
        Guid kitId,
        DateTimeOffset datePrinted,
        PrintingStatus status,
        PrintingType type,
        string? failureReason,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        BatchId = batchId;
        KitId = kitId;
        Status = status;
        Type = type;
        FailureReason = failureReason;
        DatePrinted = datePrinted;
    }
}