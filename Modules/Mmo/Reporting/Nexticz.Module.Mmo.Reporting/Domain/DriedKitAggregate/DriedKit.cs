using Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate.Events;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate;

public class DriedKit : AggregateRoot
{
    public Guid KitIdFromDrying { get; private set; }
    public Guid BatchId { get; private set; }
    public int CompletedKitsCount { get; private set; }
    public string KitCode { get; private set; }
    public string LineCode { get; private set; }
    public int ExpectedDryingTime { get; private set; }
    public DateTimeOffset DryingStarted { get; private set; }
    public DateTimeOffset DryingEnded { get; private set; }
    public DriedKitDestination Destination { get; private set; }
    public DateTimeOffset? TransferredToDryingSectionAt { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private DriedKit() {}

    public DriedKit(
        Guid kitIdFromDrying,
        Guid batchId,
        int completedKitsCount,
        string kitCode,
        string lineCode,
        int expectedDryingTime,
        DateTimeOffset dryingStarted,
        DateTimeOffset dryingEnded,
        DriedKitDestination destination,
        DateTimeOffset? transferredToDryingSectionAt,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        KitIdFromDrying = kitIdFromDrying;
        BatchId = batchId;
        CompletedKitsCount = completedKitsCount;
        KitCode = kitCode ?? throw new ArgumentNullException(nameof(kitCode));
        LineCode = lineCode ?? throw new ArgumentNullException(nameof(lineCode));

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(expectedDryingTime);

        ExpectedDryingTime = expectedDryingTime;
        DryingStarted = dryingStarted;
        DryingEnded = dryingEnded;
        Destination = destination;
        TransferredToDryingSectionAt = transferredToDryingSectionAt;
    }
    
    public void Apply(DriedKitCreatedEvent @event)
    {
        Id = @event.Id;
        KitIdFromDrying = @event.KitIdFromDrying;
        CompletedKitsCount = @event.CompletedKitsCount;
        BatchId = @event.BatchId;
        KitCode = @event.KitCode;
        LineCode = @event.LineCode;
        ExpectedDryingTime = @event.ExpectedDryingTime;
        DryingStarted = @event.DryingStarted;
        DryingEnded = @event.DryingEnded;
        Destination = @event.Destination;
        TransferredToDryingSectionAt = @event.TransferredToDryingSectionAt;
    }
}