using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity.Events;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;

public class WashingMachine : Entity
{
    public string Code { get; private set; }
    public string Note { get; private set; }
    public WashingMachineStatus Status { get; private set; }
    public int Length { get; private set; } // set in millimeter
    public int MinWidth { get; private set; } // set in millimeter
    public int MaxWidth { get; private set; } // set in millimeter
    public int MaxHeight { get; private set; } // set in millimeter
    public int MaxWaterTemperature { get; private set; } // set in degrees Celsius
    public int MaxAirTemperature { get; private set; } // set in degrees Celsius
    public int NumberOfLines { get; private set; }
    public int Speed1 { get; private set; } // set in millimeter per second (mm/s)
    public int Speed2 { get; private set; } // set in millimeter per second (mm/s)
    public int Speed3 { get; private set; } // set in millimeter per second (mm/s)
    public WashingMachineLine[] WashingMachineLines { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private WashingMachine() {}
    
    public WashingMachine(
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
        WashingMachineLine[] washingMachineLines, 
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        Note = note;
        Status = status;
        Length = length;

        if (MaxWidth < MinWidth)
        {
            throw new ArgumentException("Max width must be greater than min width", nameof(MaxWidth));
        }
        
        MinWidth = minWidth;
        MaxWidth = maxWidth;
        MaxHeight = maxHeight;
        MaxWaterTemperature = maxWaterTemperature;
        MaxAirTemperature = maxAirTemperature;
        NumberOfLines = numberOfLines;
        Speed1 = speed1;
        Speed2 = speed2;
        Speed3 = speed3;
        
        if (washingMachineLines.Length != NumberOfLines)
        {
            throw new ArgumentException("Number of lines must be equal to number of lines in washing machine", nameof(WashingMachineLines));
        }
        
        WashingMachineLines = washingMachineLines;
    }
    
    public void Apply(WashingMachineCreatedEvent @event)
    {   
        Id = @event.Id;
        Code = @event.Code;
        Note = @event.Note;
        Status = @event.Status;
        Length = @event.Length;
        MinWidth = @event.MinWidth;
        MaxWidth = @event.MaxWidth;
        MaxHeight = @event.MaxHeight;
        MaxWaterTemperature = @event.MaxWaterTemperature;
        MaxAirTemperature = @event.MaxAirTemperature;
        NumberOfLines = @event.NumberOfLines;
        Speed1 = @event.Speed1;
        Speed2 = @event.Speed2;
        Speed3 = @event.Speed3;
        WashingMachineLines = @event.WashingMachineLines;
    }
    
    public void Apply(WashingMachineUpdatedEvent @event)
    {
        Note = @event.Note;
        Status = @event.Status;
        Speed1 = @event.Speed1;
        Speed2 = @event.Speed2;
        Speed3 = @event.Speed3;
        MaxWaterTemperature = @event.MaxWaterTemperature;
        MaxAirTemperature = @event.MaxAirTemperature;
        WashingMachineLines = @event.WashingMachineLines;

        if (@event.MinWidth.HasValue)
        {
            MinWidth = @event.MinWidth.Value;
        }
    }
}