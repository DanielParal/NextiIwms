using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity.Events;

public class WashingMachineCreatedEvent(
    Guid id,
    string code,
    string note,
    WashingMachineStatus status,
    int length,
    int minWidth,
    int maxWidth,
    int maxHeight,
    int maxWaterTemperature,
    int maxAirTemperature,
    int numberOfLines,
    int speed1,
    int speed2,
    int speed3,
    WashingMachineLine[] washingMachineLines) : EventWithCode(code), IMartenEvent
{
    public Guid Id { get; } = id;
    public string Note { get; } = note;
    public WashingMachineStatus Status { get; } = status;
    public int Length { get; } = length;
    public int MinWidth { get; } = minWidth;
    public int MaxWidth { get; } = maxWidth;
    public int MaxHeight { get; } = maxHeight;
    public int MaxWaterTemperature { get; } = maxWaterTemperature;
    public int MaxAirTemperature { get; } = maxAirTemperature;
    public int NumberOfLines { get; } = numberOfLines;
    public int Speed1 { get; } = speed1;
    public int Speed2 { get; } = speed2;
    public int Speed3 { get; } = speed3;
    public WashingMachineLine[] WashingMachineLines { get; } = washingMachineLines;
}