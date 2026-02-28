using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.Manufactures.Commands.CreateManufacture;

internal class CreateManufactureCommandValidator : AbstractValidator<CreateManufactureCommand>
{
    public CreateManufactureCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(ManufactureErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(ManufactureErrors.ValidationNameIsRequired));
    }
}