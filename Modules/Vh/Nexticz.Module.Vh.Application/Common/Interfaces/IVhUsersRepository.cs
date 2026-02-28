using Nexticz.Module.Vh.Domain.VhUsers;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.VhUsers.Common.Models;


namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface IVhUsersRepository
{
    Task<VhUser?> GetVhUserByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<VhUser?> GetVhUserByUsernameAsync(string username, CancellationToken cancellationToken);

    Task<FilteredResult> GetVhUsersAsync(VhUsersFilteringParams filteringParams,
        CancellationToken cancellationToken);
}