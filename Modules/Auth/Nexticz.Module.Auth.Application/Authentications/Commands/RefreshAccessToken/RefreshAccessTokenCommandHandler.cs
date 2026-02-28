using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Auth.Contracts.Authentications;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Auth.Application.Authentications.Common;
using Nexticz.Module.Auth.Application.Common.Interfaces;

namespace Nexticz.Module.Auth.Application.Authentications.Commands.RefreshAccessToken;

public class RefreshAccessTokenCommandHandle(
    IJwtTokenGenerator jwtTokenGenerator,
    UserManager<AppUser> userManager,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RefreshAccessTokenCommand, ErrorOr<AuthenticationResponse>>
{
    public async Task<ErrorOr<AuthenticationResponse>> Handle(RefreshAccessTokenCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = jwtTokenGenerator.ValidateAccessToken(command.AccessToken, false);

        if (validationResult is null)
            return AuthenticationErrors.ValidationTokenError;

        var decodedToken = jwtTokenGenerator.DecodeJwtToken(command.AccessToken);

        try
        {
            Guid.TryParse(decodedToken?.Claims.FirstOrDefault(x => x.Type == StringHelper.Claim.Type.MagicId)!.Value,
                out var userId);

            var appUserRefreshToken = await unitOfWork.AppUserRefreshTokensRepository
                .GetAppUserRefreshTokenAsync(userId, command.RefreshToken, cancellationToken);

            if (appUserRefreshToken is null)
                return AuthenticationErrors.ValidationTokenError;

            if (appUserRefreshToken.Expiration < DateTime.UtcNow)
                return AuthenticationErrors.ValidationTokenError;

            var appUser = await userManager.Users
                .Include(x => x.AppUserRoles)!
                .ThenInclude(x => x.AppRole)
                .Include(x => x.AppUserClaims)
                .SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);

            if (appUser is null)
                throw new ArgumentNullException(nameof(appUser));
            
            if (appUser.BlockedFrom is not null && appUser.BlockedFrom < DateTimeOffset.UtcNow) 
                return AuthenticationErrors.UserIsBlockedError;

            appUser.LastActivity = DateTime.UtcNow;
            appUserRefreshToken.LastActivity = DateTime.UtcNow;

            await unitOfWork.CompleteAsync(cancellationToken);

            return new AuthenticationResponse
            {
                Id = appUser.Id,
                Username = appUser.UserName!,
                Email = appUser.Email!,
                Company = appUser.Company,
                Firsname = appUser.Firstname,
                Lastname = appUser.Lastname,
                PhoneNumber = appUser.PhoneNumber,
                AccessToken = jwtTokenGenerator.GenerateAccessToken(appUser),
                RefreshToken = appUserRefreshToken.Value
            };
        }
        catch
        {
            return AuthenticationErrors.ValidationTokenError;
        }
    }
}