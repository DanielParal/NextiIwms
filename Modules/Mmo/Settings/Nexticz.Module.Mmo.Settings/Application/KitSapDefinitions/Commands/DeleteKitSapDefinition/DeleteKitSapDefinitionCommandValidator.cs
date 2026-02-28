using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Commands.DeleteKitSapDefinition;

internal class DeleteKitSapDefinitionCommandValidator : AbstractValidator<DeleteKitSapDefinitionCommand>
{
    public DeleteKitSapDefinitionCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitSapDefinitionErrors.ValidationCodeIsRequired));
    }
}