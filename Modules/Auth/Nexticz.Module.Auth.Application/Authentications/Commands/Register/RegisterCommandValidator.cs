using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Module.Auth.Application.Authentications.Common;


namespace Nexticz.Module.Auth.Application.Authentications.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.RegisterRequest.Username)
            .NotEmpty()
            .WithState(x => new CustomErrorState(AuthenticationErrors.UsernameEmptyString));
        RuleFor(x => x.RegisterRequest.Username)
            .MinimumLength(4)
            .WithState(x => new CustomErrorState(AuthenticationErrors.UsernameMinimalLength, "4"));
        RuleFor(x => x.RegisterRequest.Password)
            .NotEmpty()
            .WithState(x => new CustomErrorState(AuthenticationErrors.PasswordEmptyString));
        RuleFor(x => x.RegisterRequest.Email)
            .EmailAddress()
            .WithState(x => new CustomErrorState(AuthenticationErrors.InvalidEmail));
    }
}