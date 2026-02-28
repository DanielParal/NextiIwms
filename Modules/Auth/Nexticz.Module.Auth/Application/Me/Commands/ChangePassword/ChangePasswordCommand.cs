using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Auth.Application.Interfaces;

namespace Nexticz.Module.Auth.Application.Me.Commands.ChangePassword;

internal record ChangePasswordCommand(string OldPassword, string NewPassword) : IRequest<ErrorOr<Success>>;

internal class ChangePasswordCommandHandler(
    ILogger<ChangePasswordCommandHandler> logger,
    ICurrentUserProvider currentUserProvider,
    IPasswordValidator passwordValidator,
    IUserWriteRepository userWriteRepository) : IRequestHandler<ChangePasswordCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Auth] [Start] [ChangePasswordCommandHandler]");
        var currentUser = currentUserProvider.GetCurrentUser();
        
        var isOldPasswordValid = await passwordValidator.IsPasswordValidAsync(currentUser.UserName, request.OldPassword, cancellationToken);

        if (!isOldPasswordValid)
        {
            logger.LogWarning("[Auth] [Error] [ChangePasswordCommandHandler] incorrect old password");
            return MeErrors.ValidationOldPasswordIsIncorrect;
        }
        
        var updatePasswordResult = await userWriteRepository.UpdateUserPasswordAsync(currentUser.UserName, request.NewPassword, cancellationToken);

        if (updatePasswordResult.IsError)
        {
            logger.LogWarning("[Auth] [Error] [ChangePasswordCommandHandler] problem updating user's password");
            return updatePasswordResult;
        }
        
        logger.LogInformation("[Auth] [End] [ChangePasswordCommandHandler]");
        return Result.Success;
    }
}