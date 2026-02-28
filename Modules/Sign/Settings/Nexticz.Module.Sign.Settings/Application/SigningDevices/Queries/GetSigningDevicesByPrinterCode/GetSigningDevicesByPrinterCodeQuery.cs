using MediatR;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDevicesByPrinterCode;

internal record GetSigningDevicesByPrinterCodeQuery(string PrinterCode) : IRequest<SigningDevice[]>; 