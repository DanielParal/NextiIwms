using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate.Events;

public class SigningDeviceCreatedEvent(Guid id, string code, string name, bool isActive, string locationCode, string printerCode)
    : EventWithCode(code), IMartenEvent
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public bool IsActive { get; } = isActive;
    public string LocationCode { get; } = locationCode;
    public string PrinterCode { get; } = printerCode;
}