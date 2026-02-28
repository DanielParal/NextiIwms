using ErrorOr;
using Nexticz.Module.Auth.Domain.UserAggregate;

namespace Nexticz.Module.Auth.Application.Interfaces;

public interface IUserWriteRepository
{
    Task<ErrorOr<Success>> CreateUserAsync(User user, string? password, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> UpdateUserAsync(User user, string? password, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> DeleteUserAsync(Guid id, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> UpdateUserPasswordAsync(string username, string password, CancellationToken cancellationToken);
}