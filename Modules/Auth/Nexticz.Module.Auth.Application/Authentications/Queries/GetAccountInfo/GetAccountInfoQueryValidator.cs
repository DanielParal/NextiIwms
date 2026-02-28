using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Module.Auth.Application.Authentications.Common;


namespace Nexticz.Module.Auth.Application.Authentications.Queries.GetAccountInfo;

public class GetAccountInfoValidator : AbstractValidator<GetAccountInfoQuery>
{
    public GetAccountInfoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithState(x => new CustomErrorState(AuthenticationErrors.UsernameEmptyString));
        RuleFor(x => x.Username)
            .MinimumLength(4)
            .WithState(x => new CustomErrorState(AuthenticationErrors.UsernameMinimalLength, "4"));
    }
}