using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Commands.CreatePackagingType;

internal class CreatePackagingTypeCommandValidator : AbstractValidator<CreatePackagingTypeCommand>
{
    public CreatePackagingTypeCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PackagingTypeErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PackagingTypeErrors.ValidationNameIsRequired));
    }
}