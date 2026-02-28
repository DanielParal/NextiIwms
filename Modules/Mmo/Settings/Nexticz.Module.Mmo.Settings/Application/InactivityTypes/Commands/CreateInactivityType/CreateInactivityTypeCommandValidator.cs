using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Commands.CreateInactivityType;

internal class CreateInactivityTypeCommandValidator : AbstractValidator<CreateInactivityTypeCommand>
{
    public CreateInactivityTypeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(InactivityTypeErrors.ValidationNameIsRequired));
    }
}