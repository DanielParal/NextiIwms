using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Commands.UpdateKitSapDefinition;

internal class UpdateKitSapDefinitionCommandValidator : AbstractValidator<UpdateKitSapDefinitionCommand>
{
    public UpdateKitSapDefinitionCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitSapDefinitionErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitSapDefinitionErrors.ValidationNameIsRequired));
    }
}