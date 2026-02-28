using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.Kits.Commands.DeleteKit;

internal class DeleteKitCommandValidator : AbstractValidator<DeleteKitCommand>
{
    public DeleteKitCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitErrors.ValidationCodeIsRequired));
    }
}