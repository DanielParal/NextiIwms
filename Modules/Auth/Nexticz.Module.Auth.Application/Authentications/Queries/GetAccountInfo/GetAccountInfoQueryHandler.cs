using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Auth.Application.Authentications.Common;
using Nexticz.Module.Auth.Contracts.Authentications;
using Nexticz.Module.Auth.Domain.AppUsers;
using AuthenticationMethodType = Nexticz.Module.Auth.Contracts.Authentications.AuthenticationMethodType;

namespace Nexticz.Module.Auth.Application.Authentications.Queries.GetAccountInfo;

public class GetAccountInfoQueryHandler(UserManager<AppUser> userManager) : IRequestHandler<GetAccountInfoQuery, ErrorOr<GetAccountInfoResponse>>
{
    public async Task<ErrorOr<GetAccountInfoResponse>> Handle(GetAccountInfoQuery request, CancellationToken cancellationToken)
    {
         var appUser = await userManager.Users.SingleOrDefaultAsync(x => x.UserName == request.Username, cancellationToken);

         if (appUser is null)
         {
             return new GetAccountInfoResponse
             {
                Username = request.Username,
                AuthMethods = [nameof(AuthenticationMethodType.UsenamePassword)]
             };
         }
         
         var userLogins = await userManager.GetLoginsAsync(appUser);

         return new GetAccountInfoResponse
         {
            Username = appUser.UserName!,
            AuthMethods = userLogins.Select(x => x.LoginProvider).ToList(),
            Registered = true
         };

    }
}