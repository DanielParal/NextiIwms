using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Auth.Application.Authentications.Common;
using Nexticz.Module.Auth.Application.Common.Interfaces;

namespace Nexticz.Module.Auth.Application.Authentications.Commands.Logout;

public class LogoutCommandHandler(
    IJwtTokenGenerator jwtTokenGenerator,
    IUnitOfWork unitOfWork) : IRequestHandler<LogoutCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var validationResult = jwtTokenGenerator.ValidateAccessToken(command.AccessToken, false);

        if (validationResult is null) return AuthenticationErrors.ValidationTokenError;

        var decodedToken = jwtTokenGenerator.DecodeJwtToken(command.AccessToken);

        try
        {
            Guid.TryParse(decodedToken?.Claims.FirstOrDefault(x => x.Type == StringHelper.Claim.Type.MagicId)!.Value,
                out var userId);

            var appUserRefreshToken = await unitOfWork.AppUserRefreshTokensRepository
                .GetAppUserRefreshTokenAsync(userId, command.RefreshToken, cancellationToken);

            if (appUserRefreshToken is null) return AuthenticationErrors.ValidationTokenError;
            var result = await unitOfWork.AppUserRefreshTokensRepository.RemoveAppUserRefreshTokenAsync(
                appUserRefreshToken,
                cancellationToken);
            if (result < 1) return AuthenticationErrors.ValidationTokenError;
        }
        catch
        {
            return AuthenticationErrors.ValidationTokenError;
        }

        return Result.Success;
    }
}