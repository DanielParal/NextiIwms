using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Nexticz.Module.Auth.Domain.AppUsers;

namespace Nexticz.Module.Auth.Application.Common.Interfaces;

public interface IAppUserService
{
    Task<ErrorOr<Success>> AddDefaultUserLoginsRolesAndPermissionsToUser(AppUser newAppUser);
    Task AddRefreshTokenToUser(AppUser createdAppUser, string refreshToken, CancellationToken cancellationToken);
    List<Error> CreateUserResultErrors(IEnumerable<IdentityError> identityErrors);
}