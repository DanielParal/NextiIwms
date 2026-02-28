using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Module.Auth.Application.Authentications.Common;

namespace Nexticz.Module.Auth.Application.Authentications.Commands.ConfirmEmail;

public class EmailConfirmationCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<EmailConfirmationCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(EmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var appUser = await userManager.FindByEmailAsync(request.Email);

        if (appUser is null)
        {
            return AuthenticationErrors.EmailConfirmationEmailError;
        }

        var result = await userManager.ConfirmEmailAsync(appUser, request.Token);

        if (!result.Succeeded)
        {
            return AuthenticationErrors.EmailConfirmationTokenError;
        }

        return Result.Success;
    }
}