using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Module.Auth.Application.Authentications.Common;
using Nexticz.Module.Auth.Application.MasstransitPublishers.EmailPublishers;

namespace Nexticz.Module.Auth.Application.Authentications.Commands.ResetPassword;

internal class ResetPasswordCommandHandler(
    UserManager<AppUser> userManager, 
    IAuthEmailPublisher authEmailPublisher)
    : IRequestHandler<ResetPasswordCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var appUser = await userManager.FindByEmailAsync(command.Email);

        if (appUser is null) return AppUserErrors.AppUserWithUsernameDoesNotExist;

        var result = await userManager.ResetPasswordAsync(appUser, command.Token, command.Password);

        if (!result.Succeeded) return AuthenticationErrors.InvalidPassword;
        
        await authEmailPublisher.PublishResetPasswordEmailAsync(command.Email, cancellationToken);
        
        return Result.Success;
    }
}