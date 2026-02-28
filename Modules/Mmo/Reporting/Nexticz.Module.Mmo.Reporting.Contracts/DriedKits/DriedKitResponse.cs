using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Reporting.Contracts.DriedKits;

public record DriedKitResponse(
    [property: Required] Guid Id,
    [property: Required] Guid BatchId,
    [property: Required] int CompletedKitsCount,
    [property: Required] string KitCode,
    [property: Required] string LineCode,
    [property: Required] int ExpectedDryingTime,
    [property: Required] DateTimeOffset DryingStarted,
    [property: Required] DateTimeOffset DryingEnded,
    [property: Required] DriedKitDestinationContract DestinationContract,
    DateTimeOffset? TransferredToDryingSectionAt);