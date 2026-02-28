using DevExtreme.AspNet.Data;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Auth.Application.Accounts.Queries.GetAccounts;

public class GetAccountsQueryHandler(UserManager<AppUser> userManager)
    : IRequestHandler<GetAccountsQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(GetAccountsQuery query, CancellationToken cancellationToken)
    {
        var filteredQuery = userManager
            .Users
            .Include(x => x.AppUserRoles!)
            .ThenInclude(x => x.AppRole)
            .Include(x => x.AppUserClaims);

        var loadOptions = FilteringHelper.CreateLoadOptionsFromFilteringParams(query.FilteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(filteredQuery, loadOptions, cancellationToken);

        loadResult.data = loadResult.data.OfType<AppUser>().Select(x => new AccountResponse
        {
            Id = x.Id,
            Username = x.UserName!,
            Firstname = x.Firstname,
            Lastname = x.Lastname,
            Company = x.Company,
            Email = x.Email!,
            PhoneNumber = x.PhoneNumber,
            Roles = x.AppUserRoles?.Select(r => r.AppRole?.Name).ToArray()!,
            Permissions = x.AppUserClaims?.Where(cl => cl.ClaimType == StringHelper.Claim.Type.MagicPermissions)
                .Select(c => c.ClaimValue).ToArray()!,
            BlockedFrom = x.BlockedFrom,
            PasswordSetuped = !string.IsNullOrWhiteSpace(x.PasswordHash),
            LastActivity = x.LastActivity
        });

        return loadResult.MapToFilteredResult();
    }
}