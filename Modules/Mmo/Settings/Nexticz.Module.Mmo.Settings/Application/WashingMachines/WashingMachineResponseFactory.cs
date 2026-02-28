using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;

namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines;

internal class WashingMachineResponseFactory
{
    public static WashingMachineResponse Create(WashingMachine washingMachine)
    {
        var washingMachineLines = washingMachine.WashingMachineLines
            .Select(x => 
                new WashingMachineLineContract(x.Code, x.IsActive, 
                    PrinterSettingsContractFactory.Create(x.PrinterSettings)))
            .ToArray();
        
        return new WashingMachineResponse(
            washingMachine.Id,
            washingMachine.Code, 
            washingMachine.Note,
            (WashingMachineStatusContract)washingMachine.Status,
            washingMachine.Length, 
            washingMachine.MinWidth, 
            washingMachine.MaxWidth, 
            washingMachine.MaxHeight, 
            washingMachine.MaxWaterTemperature,
            washingMachine.MaxAirTemperature, 
            washingMachine.NumberOfLines, 
            washingMachine.Speed1, 
            washingMachine.Speed2, 
            washingMachine.Speed3,
            washingMachineLines);
    }
}