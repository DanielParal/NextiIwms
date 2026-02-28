using ErrorOr;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Commands.StartBatchWashing;

internal record StartBatchWashingCommand(BatchContract Batch, 
    string WashingMachineCode, string LineCode, 
    int RequestedPackagingSpeed, SpeedLevel RequestedPackagingSpeedLevel,
    DateTimeOffset DateStarted, DateTimeOffset? PreviousBatchLastKitEndDate,
    bool ShouldFirstKitStartAfterPreviousBatchLastKitEndDate) : IWashingCommand<ErrorOr<Success>>;