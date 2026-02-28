using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrinterByCode;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Printers.Commands.CreatePrinter;

internal class CreatePrinterCommandHandler(
        ILogger<CreatePrinterCommandHandler> logger,
        ISettingsUnitOfWork unitOfWork,
        ISender sender
    ) : IRequestHandler<CreatePrinterCommand, ErrorOr<Printer>>
{
    public async Task<ErrorOr<Printer>> Handle(CreatePrinterCommand request, CancellationToken cancellationToken)
    {
        var existingPrinter = await sender.Send(new GetPrinterByCodeQuery(request.Code), cancellationToken);

        if (existingPrinter.HasValue())
        {
            logger.LogInformation("Sign - Object {ObjectName} with code: {Code} already exists. Nothing to create.",
                nameof(Printer), request.Code);
            return PrinterErrors.ValidationCodeAlreadyExists;
        }
            
        var printer = new Printer(request.Code, request.Name, request.Ip);
        var printerCreatedEvent =
            new PrinterCreatedEvent(printer.Id, printer.Code, printer.Name, request.Ip);

        unitOfWork.StartStream<PrinterCreatedEvent, Printer>(printer.Id, printerCreatedEvent);
            
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code} created.",
            nameof(Printer), request.Code);
        return printer;
    }
}