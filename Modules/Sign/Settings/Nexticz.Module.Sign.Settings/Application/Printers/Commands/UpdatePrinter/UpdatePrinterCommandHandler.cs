using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrinterByCode;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Printers.Commands.UpdatePrinter;

internal class UpdatePrinterCommandHandler(
    ILogger<UpdatePrinterCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender) 
    : IRequestHandler<UpdatePrinterCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdatePrinterCommand request, CancellationToken cancellationToken)
    {
        var printer = await sender.Send(new GetPrinterByCodeQuery(request.Code), cancellationToken);

        if (printer.IsError)
        {
            logger.LogWarning("Sign - Did not find object {ObjectName} with code: {Code}. Nothing to update", 
                nameof(Printer), request.Code);
            return PrinterErrors.ValidationCodeDoesNotExist;
        }
        
        var printerUpdatedEvent = new PrinterUpdatedEvent(printer.Value.Id, request.Code, request.Name, request.Ip);
        unitOfWork.AppendEvent(printer.Value.Id, printerUpdatedEvent);
        
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code}, name: {Name} updated.",
            nameof(Printer), request.Code, request.Name);
        return Result.Updated;
    }
}