using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.Manufactures.Commands.UpdateManufacture;

internal class UpdateManufactureCommandValidator : AbstractValidator<UpdateManufactureCommand>
{
    public UpdateManufactureCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(ManufactureErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(ManufactureErrors.ValidationNameIsRequired));
    }
}