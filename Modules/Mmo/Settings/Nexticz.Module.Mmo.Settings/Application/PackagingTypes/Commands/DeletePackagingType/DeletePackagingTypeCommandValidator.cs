using ErrorOr;
using FluentValidation;
using MediatR;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Commands.DeletePackagingType;

internal class DeletePackagingTypeCommandValidator : AbstractValidator<DeletePackagingTypeCommand>
{
    public DeletePackagingTypeCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PackagingTypeErrors.ValidationCodeIsRequired));
    }
}