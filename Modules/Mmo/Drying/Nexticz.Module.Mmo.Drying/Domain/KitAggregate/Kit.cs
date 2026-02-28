using MassTransit;
using Nexticz.Module.Mmo.Drying.Domain.KitAggregate.Events;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Drying.Domain.KitAggregate;

public class Kit : AggregateRoot
{
    public Guid BatchId { get; private set; }
    public int GlobalKitsCount { get; private set; }
    public string KitCode { get; private set; }
    public string LineCode { get; private set; }
    public int ExpectedDryingTime { get; private set; }
    public DateTimeOffset DryingStarted { get; private set; }
    public DateTimeOffset? DryingEnded { get; private set; }
    public KitDestination Destination { get; private set; }
    public DateTimeOffset? TransferredToDryingSectionAt { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Kit() {}

    public Kit(
        Guid batchId,
        int globalKitsCount,
        string kitCode,
        string lineCode,
        int expectedDryingTime,
        DateTimeOffset dryingStarted,
        KitDestination destination,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        BatchId = batchId;
        GlobalKitsCount = globalKitsCount;
        KitCode = kitCode ?? throw new ArgumentNullException(nameof(kitCode));
        LineCode = lineCode ?? throw new ArgumentNullException(nameof(lineCode));

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(expectedDryingTime);

        ExpectedDryingTime = expectedDryingTime;
        DryingStarted = dryingStarted;
        Destination = destination;
        DryingEnded = null;
        TransferredToDryingSectionAt = null;
    }

    public void FinishDrying(DateTimeOffset dryingEnded)
    {
        DryingEnded = dryingEnded;
    }

    public void TransferToDryingSection(DateTimeOffset transferredToDryingSectionAt)
    {
        Destination = KitDestination.DryingSection;
        TransferredToDryingSectionAt = transferredToDryingSectionAt;
    }
    
    public void Apply(KitCreatedEvent @event)
    {
        Id = @event.Id;
        GlobalKitsCount = @event.GlobalKitsCount;
        BatchId = @event.BatchId;
        KitCode = @event.KitCode;
        LineCode = @event.LineCode;
        ExpectedDryingTime = @event.ExpectedDryingTime;
        DryingStarted = @event.DryingStarted;
        DryingEnded = null;
        Destination = KitDestination.CompletingSection;
    }

    public void Apply(KitToDryingSectionTransferredEvent @event)
    {
        TransferToDryingSection(@event.TransferredAt);
    }
}