using Nexticz.Module.Auth.Domain.AppUserRefreshTokens;

namespace Nexticz.Module.Auth.Application.Common.Interfaces;

public interface IAppUserRefreshTokensRepository
{
    Task AddAppUserRefreshTokenAsync(AppUserRefreshToken appUserRefreshToken, CancellationToken cancellationToken);

    Task<AppUserRefreshToken?> GetAppUserRefreshTokenAsync(Guid userId, string tokenValue,
        CancellationToken cancellationToken);

    Task<int> RemoveAppUserRefreshTokenAsync(AppUserRefreshToken appUserRefreshToken,
        CancellationToken cancellationToken);

    Task<List<AppUserRefreshToken>> GetAppUserRefreshTokensByUserIdAsync(Guid userId,
        CancellationToken cancellationToken);

    Task<AppUserRefreshToken?> GetAppUserRefreshTokenByIdAsync(Guid id, CancellationToken cancellationToken);
}