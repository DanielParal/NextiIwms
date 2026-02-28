using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;

namespace Nexticz.Module.Sign.Settings.Application.Printers.Commands.UpdatePrinter;

internal class UpdatePrinterCommandValidator : AbstractValidator<UpdatePrinterCommand>
{
    public UpdatePrinterCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PrinterErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PrinterErrors.ValidationNameIsRequired));
        
        RuleFor(x => x.Ip)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PrinterErrors.ValidationIpIsRequired));
    }
}