
namespace Nexticz.Module.Mmo.Drying.Contracts.Kits;

public record KitContract(
    Guid KitId,
    Guid BatchId,
    int CompletedKitsCount,
    string KitCode,
    string LineCode,
    int ExpectedDryingTime,
    DateTimeOffset DryingStarted,
    DateTimeOffset DryingEnded,
    KitDestinationContract Destination,
    DateTimeOffset? TransferredToDryingSectionAt);