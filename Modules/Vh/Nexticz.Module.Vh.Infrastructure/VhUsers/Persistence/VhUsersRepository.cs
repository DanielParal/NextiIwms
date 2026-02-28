using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.VhUsers.Common.Models;
using Nexticz.Module.Vh.Domain.VhUsers;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.VhUsers.Persistence;

public class VhUsersRepository(DataContext context) : IVhUsersRepository
{
    public async Task<VhUser?> GetVhUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.VhUsers
            .Include(x => x.Centers)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<VhUser?> GetVhUserByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await context.VhUsers
            .FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
    }

    public async Task<FilteredResult> GetVhUsersAsync(VhUsersFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.VhUsers
            .Where(x => x.Active == true)
            .Include(x => x.Centers);

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }
}