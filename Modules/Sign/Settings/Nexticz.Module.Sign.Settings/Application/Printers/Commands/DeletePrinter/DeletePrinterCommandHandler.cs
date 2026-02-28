using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrinterByCode;
using Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDevicesByPrinterCode;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Printers.Commands.DeletePrinter;

internal class DeletePrinterCommandHandler(
    ILogger<DeletePrinterCommandHandler> logger,
    ISender sender,
    ISettingsUnitOfWork unitOfWork
) 
    : IRequestHandler<DeletePrinterCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeletePrinterCommand request, CancellationToken cancellationToken)
    {
        var printer = await sender.Send(new GetPrinterByCodeQuery(request.Code), cancellationToken);

        if (printer.IsError)
        {
            logger.LogInformation("Sign - Did not find object {ObjectName} with code: {Code}. Nothing to delete.", nameof(Printer), request.Code);
            return PrinterErrors.ValidationCodeDoesNotExist;
        }
        
        var singingDevices = await sender.Send(new GetSigningDevicesByPrinterCodeQuery(printer.Value.Code), cancellationToken);
        if (singingDevices.Length > 0)
        {
            var signingDevicesCodes = string.Join(", ", singingDevices.Select(x => x.Code));
            logger.LogInformation("Sign - we cannot delete {ObjectName} with code: {Code} because it is used by signing devices: {SigningDevicesCodes}.",
                nameof(Printer), printer.Value.Code, signingDevicesCodes);
            return PrinterErrors.ValidationCodeIsUsedInSigningDevices(signingDevicesCodes);
        }
        
        var printerDeletedEvent = new PrinterDeletedEvent(printer.Value.Id, printer.Value.Code);
        unitOfWork.AppendEvent(printer.Value.Id, printerDeletedEvent);
        
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code} deleted.",
            nameof(Printer), printer.Value.Code);
        return Result.Deleted;
    }
}