using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Module.Auth.Application.Authentications.Common;


namespace Nexticz.Module.Auth.Application.Authentications.Queries.Login;

public class LoginQueryValidator : AbstractValidator<LoginQuery>
{
    public LoginQueryValidator()
    {
        RuleFor(x => x.LoginRequest.Username)
            .NotEmpty()
            .WithState(x => new CustomErrorState(AuthenticationErrors.UsernameEmptyString));
        RuleFor(x => x.LoginRequest.Username)
            .MinimumLength(4)
            .WithState(x => new CustomErrorState(AuthenticationErrors.UsernameMinimalLength, "4"));
        RuleFor(x => x.LoginRequest.Password)
            .NotEmpty()
            .WithState(x => new CustomErrorState(AuthenticationErrors.PasswordEmptyString));
    }
}