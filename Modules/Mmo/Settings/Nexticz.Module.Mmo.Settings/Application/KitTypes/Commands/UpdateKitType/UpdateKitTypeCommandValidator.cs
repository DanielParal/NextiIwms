using FluentValidation;
using Nexticz.Module.Mmo.Settings.Application.Manufactures;
using Nexticz.Module.Mmo.Settings.Application.Manufactures.Commands.UpdateManufacture;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.KitTypes.Commands.UpdateKitType;

internal class UpdateKitTypeCommandValidator : AbstractValidator<UpdateKitTypeCommand>
{
    public UpdateKitTypeCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitTypeErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitTypeErrors.ValidationNameIsRequired));
    }
}