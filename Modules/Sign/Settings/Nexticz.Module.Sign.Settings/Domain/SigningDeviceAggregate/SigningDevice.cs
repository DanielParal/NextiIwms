using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate.Events;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;

public class SigningDevice : AggregateRoot
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public string LocationCode { get; private set; }
    public string PrinterCode { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private SigningDevice() {}
    
    public SigningDevice(
        string code, 
        string name, 
        bool isActive, 
        string locationCode, 
        string printerCode,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        Name = name;
        IsActive = isActive;
        LocationCode = locationCode;
        PrinterCode = printerCode;
    }

    public void Apply(SigningDeviceCreatedEvent @event)
    {
        Code = @event.Code;
        Name = @event.Name;
        IsActive = @event.IsActive;
        LocationCode = @event.LocationCode;
        PrinterCode = @event.PrinterCode;
    }
    
    public void Apply(SigningDeviceUpdatedEvent @event)
    {
        Name = @event.Name;
        IsActive = @event.IsActive;
        LocationCode = @event.LocationCode;
        PrinterCode = @event.PrinterCode;
    }
}