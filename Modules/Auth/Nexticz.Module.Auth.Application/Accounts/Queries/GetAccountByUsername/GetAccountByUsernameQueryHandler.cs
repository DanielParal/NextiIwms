using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Auth.Application.Accounts.Queries.GetAccountByUsername;

public class GetAccountByUsernameQueryHandler(UserManager<AppUser> userManager)
    : IRequestHandler<GetAccountByUsernameQuery, ErrorOr<AccountResponse>>
{
    public async Task<ErrorOr<AccountResponse>> Handle(GetAccountByUsernameQuery query,
        CancellationToken cancellationToken)
    {
        var account = await userManager
            .Users
            .Include(x => x.AppUserRoles!)
            .ThenInclude(x => x.AppRole)
            .Include(x => x.AppUserClaims)
            .Where(x => x.UserName == query.Username)
            .Select(x => new AccountResponse
            {
                Id = x.Id,
                Username = x.UserName!,
                Firstname = x.Firstname,
                Lastname = x.Lastname,
                Company = x.Company,
                Email = x.Email!,
                PhoneNumber = x.PhoneNumber,
                Roles = x.AppUserRoles!.Select(r => r.AppRole!.Name).ToArray()!,
                Permissions = x.AppUserClaims!.Where(cl => cl.ClaimType == StringHelper.Claim.Type.MagicPermissions)
                    .Select(c => c.ClaimValue).ToArray()!,
                BlockedFrom = x.BlockedFrom,
                PasswordSetuped = !string.IsNullOrWhiteSpace(x.PasswordHash),
                LastActivity = x.LastActivity
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (account is null) return AppUserErrors.AppUserWithUsernameDoesNotExist;

        return account;
    }
}