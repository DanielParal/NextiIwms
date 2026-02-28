using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Auth.Domain.UserAggregate;

namespace Nexticz.Module.Auth.Application.Interfaces;

public interface IUserReadOnlyRepository
{
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken);
    Task<FilteredResult<User>> GetUsersAsync(BaseFilteringParams filteringParams, CancellationToken cancellationToken);
}