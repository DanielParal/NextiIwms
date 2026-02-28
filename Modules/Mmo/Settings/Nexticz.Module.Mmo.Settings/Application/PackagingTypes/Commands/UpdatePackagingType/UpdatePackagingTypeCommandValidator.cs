using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Commands.UpdatePackagingType;

internal class UpdatePackagingTypeCommandValidator : AbstractValidator<UpdatePackagingTypeCommand>
{
    public UpdatePackagingTypeCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PackagingTypeErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PackagingTypeErrors.ValidationNameIsRequired));
    }
}