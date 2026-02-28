using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Commands.CreateSigningDevice;

internal class CreateSigningDeviceCommandValidator : AbstractValidator<CreateSigningDeviceCommand>
{
    public CreateSigningDeviceCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(SigningDeviceErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(SigningDeviceErrors.ValidationNameIsRequired));
        
        RuleFor(x => x.LocationCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(SigningDeviceErrors.ValidationLocationCodeIsRequired));
        
        RuleFor(x => x.PrinterCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(SigningDeviceErrors.ValidationPrinterCodeIsRequired));
    }
}