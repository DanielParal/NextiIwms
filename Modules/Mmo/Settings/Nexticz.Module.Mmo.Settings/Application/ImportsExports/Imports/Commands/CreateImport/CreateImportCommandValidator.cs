using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.ImportsExports.Imports.Commands.CreateImport;

internal class CreateImportCommandValidator : AbstractValidator<CreateImportCommand>
{
    public CreateImportCommandValidator()
    {
        RuleFor(x => x.FormFile)
            .NotEmpty()
            .WithState(x => new CustomErrorState(ImportErrors.ValidationFormFileIsRequired));
    }
}