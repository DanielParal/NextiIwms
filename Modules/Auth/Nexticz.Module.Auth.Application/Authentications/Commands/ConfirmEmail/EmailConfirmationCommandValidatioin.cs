using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Module.Auth.Application.Authentications.Common;


namespace Nexticz.Module.Auth.Application.Authentications.Commands.ConfirmEmail;

public class EmailConfirmationCommandValidatioin : AbstractValidator<EmailConfirmationCommand>
{
    public EmailConfirmationCommandValidatioin()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithState(x => new CustomErrorState(AuthenticationErrors.EmailConfirmationTokenError));
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithState(x => new CustomErrorState(AuthenticationErrors.EmailConfirmationTokenError));
    }
}