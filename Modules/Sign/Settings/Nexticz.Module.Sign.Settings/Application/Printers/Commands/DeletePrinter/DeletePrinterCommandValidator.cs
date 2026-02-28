using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;

namespace Nexticz.Module.Sign.Settings.Application.Printers.Commands.DeletePrinter;

internal class DeletePrinterCommandValidator : AbstractValidator<DeletePrinterCommand>
{
    public DeletePrinterCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PrinterErrors.ValidationCodeIsRequired));
    }
}