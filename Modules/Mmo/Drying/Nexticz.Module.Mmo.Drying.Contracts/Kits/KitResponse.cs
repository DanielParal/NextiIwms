using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Drying.Contracts.Kits;

public record KitResponse(
    [property: Required] Guid Id,
    [property: Required] int CompletedKitsCount,
    [property: Required] string KitCode,
    [property: Required] int ExpectedDryingTime,
    [property: Required] DateTimeOffset DryingStarted,
    [property: Required] string RemainingDryingTime,
    [property: Required] KitDestinationContract Destination,
    DateTimeOffset? TransferredToDryingSectionAt);