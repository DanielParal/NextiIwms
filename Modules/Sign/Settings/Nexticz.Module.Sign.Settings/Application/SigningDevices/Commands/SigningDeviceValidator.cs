using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.Locations.Queries.GetLocationByCode;
using Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrinterByCode;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Commands;

internal static class SigningDeviceValidator
{
    public static async Task<ErrorOr<SigningDeviceValidatorResult>> ValidateAsync(string locationCode, string printerCode, ISender sender, ILogger logger, CancellationToken cancellationToken)
    {
        var existingLocation = await sender.Send(new GetLocationByCodeQuery(locationCode), cancellationToken);
        if (existingLocation.IsError)
        {
            logger.LogInformation("Sign - location with code: {Code} does not exists. Nothing to create.",
                locationCode);
            return SigningDeviceErrors.ValidationLocationDoesNotExists;
        } 
        
        var existingPrinter = await sender.Send(new GetPrinterByCodeQuery(printerCode), cancellationToken);
        if (existingPrinter.IsError)
        {
            logger.LogInformation("Sign - printer with code: {Code} does not exists. Nothing to create.",
                printerCode);
            return SigningDeviceErrors.ValidationPrinterDoesNotExists;
        }

        return new SigningDeviceValidatorResult(existingLocation.Value.Code, existingPrinter.Value.Code);
    }
}

internal record SigningDeviceValidatorResult(string LocationCode, string PrinterCode);