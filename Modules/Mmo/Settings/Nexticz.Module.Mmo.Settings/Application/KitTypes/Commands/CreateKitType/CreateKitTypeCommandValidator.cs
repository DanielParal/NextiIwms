using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.KitTypes.Commands.CreateKitType;

internal class CreateKitTypeCommandValidator : AbstractValidator<CreateKitTypeCommand>
{
    public CreateKitTypeCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitTypeErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitTypeErrors.ValidationNameIsRequired));
    }
}