using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Auth.Application.Common.Interfaces;
using Nexticz.Module.Auth.Domain.AppUserRefreshTokens;
using Nexticz.Module.Auth.Infrastructure.Common.Persistence;

namespace Nexticz.Module.Auth.Infrastructure.AppUserRefreshTokens.Persistence;

public class AppUserRefreshTokensRepository(DataContext context) : IAppUserRefreshTokensRepository
{
    public async Task AddAppUserRefreshTokenAsync(AppUserRefreshToken appUserRefreshToken,
        CancellationToken cancellationToken)
    {
        await context.AppUserRefreshTokens.AddAsync(appUserRefreshToken, cancellationToken);
    }

    public async Task<AppUserRefreshToken?> GetAppUserRefreshTokenAsync(Guid userId, string tokenValue,
        CancellationToken cancellationToken)
    {
        return await context.AppUserRefreshTokens
            .Where(x => x.UserId == userId && x.Value == tokenValue)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> RemoveAppUserRefreshTokenAsync(AppUserRefreshToken appUserRefreshToken,
        CancellationToken cancellationToken)
    {
        context.AppUserRefreshTokens.Remove(appUserRefreshToken);
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<AppUserRefreshToken>> GetAppUserRefreshTokensByUserIdAsync(Guid userId,
        CancellationToken cancellationToken)
    {
        return await context.AppUserRefreshTokens
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<AppUserRefreshToken?> GetAppUserRefreshTokenByIdAsync(Guid id,
        CancellationToken cancellationToken)
    {
        return await context.AppUserRefreshTokens
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}