using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Commands.DeletePackagingCirculation;

internal class DeletePackagingCirculationCommandValidator : AbstractValidator<DeletePackagingCirculationCommand>
{
    public DeletePackagingCirculationCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PackagingCirculationErrors.ValidationCodeIsRequired));
    }
}