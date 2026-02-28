using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Commands.CreateKitSapDefinition;

internal class CreateKitSapDefinitionCommandValidator : AbstractValidator<CreateKitSapDefinitionCommand>
{
    public CreateKitSapDefinitionCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitSapDefinitionErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitSapDefinitionErrors.ValidationNameIsRequired));
    }
}