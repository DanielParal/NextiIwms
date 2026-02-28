using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Commands.UpdateInactivityType;

internal class UpdateInactivityTypeCommandValidator : AbstractValidator<UpdateInactivityTypeCommand>
{
    public UpdateInactivityTypeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(InactivityTypeErrors.ValidationNameIsRequired));
    }
}