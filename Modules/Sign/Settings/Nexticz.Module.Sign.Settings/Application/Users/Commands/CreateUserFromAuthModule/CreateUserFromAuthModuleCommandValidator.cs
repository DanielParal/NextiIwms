using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;

namespace Nexticz.Module.Sign.Settings.Application.Users.Commands.CreateUserFromAuthModule;

internal class CreateUserFromAuthModuleCommandValidator : AbstractValidator<CreateUserFromAuthModuleCommand>
{
    public CreateUserFromAuthModuleCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithState(x => new CustomErrorState(UserErrors.ValidationUserNameIsRequired));
    }
}