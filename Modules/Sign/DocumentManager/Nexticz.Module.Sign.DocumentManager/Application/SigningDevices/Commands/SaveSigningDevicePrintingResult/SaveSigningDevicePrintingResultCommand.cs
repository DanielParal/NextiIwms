using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.SaveSigningDevicePrintingResult;

internal record SaveSigningDevicePrintingResultCommand(Guid SigningDeviceId, string SigningDeviceCode, ErrorOr<Success> PrintingResult, DateTimeOffset PrintedAt, DocumentPrintJob[] DocumentPrintJobs) : IDocumentManagerCommand<ErrorOr<Success>>;