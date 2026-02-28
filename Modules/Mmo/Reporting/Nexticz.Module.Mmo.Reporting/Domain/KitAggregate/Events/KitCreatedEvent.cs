using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Reporting.Domain.SpecialInformationEntity;
using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Reporting.Domain.KitAggregate.Events;

public record KitCreatedEvent(
    Guid Id,
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
    string DeclaredBy,
    int GlobalKitsCount,
    double Efficiency,
    TimeSpan RealTimeKitDuration,
    TimeSpan OptimalKitDuration,
    DateTimeOffset WashingStarted,
    DateTimeOffset WashingEnded,
    DateTimeOffset CreatedAt,
    SpecialInformation? SpecialInformation) : IMartenEvent;