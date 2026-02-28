using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.CreateSigningDevice;

internal record CreateSigningDeviceCommand(string Code, string Name, bool IsActive, string PrinterCode) : IDocumentManagerCommand<ErrorOr<SigningDevice>>;