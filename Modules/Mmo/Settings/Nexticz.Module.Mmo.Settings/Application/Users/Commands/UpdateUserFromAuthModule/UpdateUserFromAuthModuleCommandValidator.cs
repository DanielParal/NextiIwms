using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.Users.Commands.UpdateUserFromAuthModule;

internal class UpdateUserFromAuthModuleCommandValidator : AbstractValidator<UpdateUserFromAuthModuleCommand>
{
    public UpdateUserFromAuthModuleCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithState(x => new CustomErrorState(UserErrors.ValidationUserNameIsRequired));
    }
}