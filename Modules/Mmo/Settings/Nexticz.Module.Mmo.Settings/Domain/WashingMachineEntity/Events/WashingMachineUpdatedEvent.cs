using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity.Events;

public record WashingMachineUpdatedEvent(
    Guid Id,
    string Note,
    WashingMachineStatus Status,
    int Speed1,
    int Speed2,
    int Speed3,
    int MaxWaterTemperature,
    int MaxAirTemperature,
    int? MinWidth,
    WashingMachineLine[] WashingMachineLines) : IMartenEvent;