using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Commands.DeletePackaging;

internal class DeletePackagingCommandValidator : AbstractValidator<DeletePackagingCommand>
{
    public DeletePackagingCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationCodeIsRequired));
    }
}