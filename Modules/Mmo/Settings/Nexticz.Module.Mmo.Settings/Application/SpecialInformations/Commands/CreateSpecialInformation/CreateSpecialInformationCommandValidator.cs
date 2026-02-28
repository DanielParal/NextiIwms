using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Commands.CreateSpecialInformation;

internal class CreateSpecialInformationCommandValidator : AbstractValidator<CreateSpecialInformationCommand>
{
    public CreateSpecialInformationCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithState(x => new CustomErrorState(SpecialInformationErrors.ValidationTitleIsRequired));
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithState(x => new CustomErrorState(SpecialInformationErrors.ValidationDescriptionIsRequired));
    }
}