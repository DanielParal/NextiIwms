using FluentValidation;
using Nexticz.Module.Mmo.Settings.Application.Manufactures.Commands.DeleteManufacture;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.KitTypes.Commands.DeleteKitType;

internal class DeleteKitTypeCommandValidator : AbstractValidator<DeleteKitTypeCommand>
{
    public DeleteKitTypeCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitTypeErrors.ValidationCodeIsRequired));
    }
}