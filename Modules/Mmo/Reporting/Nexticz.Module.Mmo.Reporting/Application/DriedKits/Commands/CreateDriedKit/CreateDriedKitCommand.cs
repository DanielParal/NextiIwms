
using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.DriedKits.Commands.CreateDriedKit;

internal record CreateDriedKitCommand(
    Guid KitIdFromDrying, Guid BatchId, int CompletedKitsCount, string KitCode, string LineCode, int ExpectedDryingTime, 
    DateTimeOffset DryingStarted, DateTimeOffset DryingEnded, DriedKitDestination Destination, DateTimeOffset? TransferredToDryingSectionAt) 
    : IReportingCommand<ErrorOr<Success>>;