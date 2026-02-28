using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Auth.Contracts.Authentications;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Auth.Application.Authentications.Common;
using Nexticz.Module.Auth.Application.Common.Interfaces;

namespace Nexticz.Module.Auth.Application.Authentications.Queries.GetLoggedUserInfo;

public class GetLoggedUserInfoQueryHandler(
    IJwtTokenGenerator jwtTokenGenerator,
    UserManager<AppUser> userManager,
    IUnitOfWork unitOfWork) : IRequestHandler<GetLoggedUserInfoQuery, ErrorOr<AuthenticationResponse>>
{
    public async Task<ErrorOr<AuthenticationResponse>> Handle(GetLoggedUserInfoQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = jwtTokenGenerator.ValidateAccessToken(query.AccessToken, false);

        if (validationResult is null) 
            return AuthenticationErrors.ValidationTokenError;

        var decodedToken = jwtTokenGenerator.DecodeJwtToken(query.AccessToken);

        try
        {
            Guid.TryParse(decodedToken?.Claims.FirstOrDefault(x => x.Type == StringHelper.Claim.Type.MagicId)!.Value,
                out var userId);

            var appUserRefreshToken = await unitOfWork.AppUserRefreshTokensRepository
                .GetAppUserRefreshTokenAsync(userId, query.RefreshToken, cancellationToken);

            if (appUserRefreshToken is null) return AuthenticationErrors.ValidationTokenError;

            var appUser = await userManager.Users
                .Include(x => x.AppUserRoles)!
                .ThenInclude(x => x.AppRole)
                .Include(x => x.AppUserClaims)
                .SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);

            if (appUser is null)
                throw new ArgumentNullException(nameof(appUser));

            // appUser.LastActivity = DateTime.UtcNow;
            // appUserRefreshToken.LastActivity = DateTime.UtcNow;
            //
            // await userManager.UpdateAsync(appUser);
            //
            // unitOfWork.Update(appUserRefreshToken);
            // await unitOfWork.CompleteAsync(cancellationToken);

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