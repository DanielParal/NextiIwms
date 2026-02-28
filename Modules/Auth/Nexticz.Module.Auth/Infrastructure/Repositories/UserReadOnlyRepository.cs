using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Auth.Application.Interfaces;
using Nexticz.Module.Auth.Domain.UserAggregate;
using Nexticz.Module.Auth.Infrastructure.Identity;

namespace Nexticz.Module.Auth.Infrastructure.Repositories;

internal class UserReadOnlyRepository(UserManager<AppUser> userManager) : IUserReadOnlyRepository
{
    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var appUser = await UserQuery
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        
        if (appUser == null)
            return null;
        
        return AppUserMapper.ToDomain(appUser);
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        
        var appUser = await UserQuery
            .FirstOrDefaultAsync(x => x.Email != null && x.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase), cancellationToken);
        
        if (appUser == null)
            return null;
        
        return AppUserMapper.ToDomain(appUser);
    }

    public async Task<User?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        var appUser = await UserQuery
            .FirstOrDefaultAsync(x => x.UserName != null && x.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase), cancellationToken);
        
        if (appUser == null)
            return null;
        
        return AppUserMapper.ToDomain(appUser);
    }

    public async Task<FilteredResult<User>> GetUsersAsync(BaseFilteringParams filteringParams, CancellationToken cancellationToken)
    {
        var filteredQuery = UserQuery;

        var loadOptions = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(filteredQuery, loadOptions, cancellationToken);

        loadResult.data = loadResult.data.OfType<AppUser>().Select(AppUserMapper.ToDomain);
        
        return loadResult.MapToFilteredResult<User>();
    }
    
    private IIncludableQueryable<AppUser,ICollection<AppUserApiKey>> UserQuery => 
        userManager
            .Users
            .Include(x => x.AppUserRoles!)
            .ThenInclude(x => x.AppRole)
            .Include(x => x.AppUserClaims)
            .Include(x => x.AppUserApiKeys);

}