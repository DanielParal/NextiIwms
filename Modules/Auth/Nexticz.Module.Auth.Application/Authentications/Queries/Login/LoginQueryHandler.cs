using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Auth.Contracts.Authentications;
using Nexticz.Module.Auth.Domain.AppUserRefreshTokens;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Auth.Application.Authentications.Common;
using Nexticz.Module.Auth.Application.Common.Interfaces;

namespace Nexticz.Module.Auth.Application.Authentications.Queries.Login;

public class LoginQueryHandler(
    UserManager<AppUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator,
    ICurrentUserProvider currentUserProvider,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResponse>>
{
    public async Task<ErrorOr<AuthenticationResponse>> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        var appUser = await userManager.Users
            .Include(x => x.AppUserRoles)!
            .ThenInclude(x => x.AppRole)
            .Include(x => x.AppUserClaims)
            .SingleOrDefaultAsync(x => x.UserName == query.LoginRequest.Username, cancellationToken);

        if (appUser is null) return 
            AuthenticationErrors.InvalidUsername;

        if (appUser.BlockedFrom is not null && appUser.BlockedFrom < DateTimeOffset.UtcNow) 
            return AuthenticationErrors.UserIsBlockedError;

        var checkPasswordResult = await userManager.CheckPasswordAsync(appUser, query.LoginRequest.Password);

        if (!checkPasswordResult) return 
            AuthenticationErrors.InvalidPassword;

        var refreshToken = jwtTokenGenerator.GenerateRefreshToken();

        await AddRefreshTokenToUser(appUser, refreshToken, cancellationToken);

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
            RefreshToken = refreshToken
        };
    }

    private async Task AddRefreshTokenToUser(AppUser createdAppUser, string refreshToken,
        CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var appUserRefreshToken = new AppUserRefreshToken
        {
            UserId = createdAppUser.Id, Value = refreshToken, UserDeviceInfo = currentUser.UserDeviceInfo,
            Expiration = DateTime.UtcNow.AddYears(1)
        };

        await unitOfWork.AppUserRefreshTokensRepository.AddAppUserRefreshTokenAsync(appUserRefreshToken,
            cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
    }
}