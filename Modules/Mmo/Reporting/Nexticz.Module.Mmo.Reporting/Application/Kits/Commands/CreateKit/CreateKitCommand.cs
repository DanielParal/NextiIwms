using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Domain.SpecialInformationEntity;
using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Kits.Commands.CreateKit;

internal record CreateKitCommand(
    Guid KitId,
    Guid? SisterKitId,
    Guid ShiftId,
    Guid BatchId,
    Guid? SisterBatchId,
    string WashingMachineCode,
    string LineCode,
    int WashingMachineSpeed,
    SpeedLevel WashingMachineSpeedLevel,
    int KitOrderId,
    int TotalPlannedKitsCountInBatch,
    string KitCode,
    string KitNumber,
    string KitSapDefinitionCode,
    string KitSapDefinitionName,
    string? SapBarcode,
    string PackagingCode,
    int OptimalPackagingSpeedOnWashingMachine,
    SpeedLevel OptimalPackagingSpeedOnWashingMachineLevel,
    string DefiningPackagingCode,
    string WorkerName,
    int GlobalKitsCount,
    TimeSpan RealTimeKitDuration,
    TimeSpan OptimalKitDuration,
    DateTimeOffset WashingStarted,
    DateTimeOffset WashingEnded,
    SpecialInformation? SpecialInformation) : IReportingCommand<ErrorOr<Success>>;