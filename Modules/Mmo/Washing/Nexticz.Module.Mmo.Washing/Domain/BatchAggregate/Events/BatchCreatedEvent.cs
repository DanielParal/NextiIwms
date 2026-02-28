using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Washing.Domain.SpecialInformationEntity;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;

public record BatchCreatedEvent(
    Guid Id,
    Guid? SisterBatchId,
    string WashingMachineCode,
    int WashingMachineLength,
    string LineCode,
    string KitCode,
    string KitNumber,
    int PlannedKitsCount,
    string KitSapDefinitionCode,
    string KitSapDefinitionName,
    string SapBarcode,
    string PackagingCode,
    int OptimalPackagingSpeedOnCurrentMachine,
    SpeedLevel OptimalPackagingSpeedOnCurrentMachineLevel,
    int PackagingSpeed,
    SpeedLevel PackagingSpeedLevel,
    decimal PackagingHeight,
    string DefiningPackagingCode,
    TimeSpan OptimalKitDuration,
    DateTimeOffset DateActivated,
    DateTimeOffset? PreviousBatchLastKitEndDate,
    bool ShouldFirstKitStartAfterPreviousBatchLastKitEndDate,
    SpecialInformation? SpecialInformation) : IMartenEvent;