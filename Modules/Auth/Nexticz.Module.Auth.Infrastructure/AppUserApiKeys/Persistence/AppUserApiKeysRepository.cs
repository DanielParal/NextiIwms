using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Auth.Application.Common.Interfaces;
using Nexticz.Module.Auth.Domain.AppUserApiKeys;
using Nexticz.Module.Auth.Infrastructure.Common.Persistence;

namespace Nexticz.Module.Auth.Infrastructure.AppUserApiKeys.Persistence;

public class AppUserApiKeysRepository(DataContext context) : IAppUserApiKeysRepository
{
    public async Task<AppUserApiKey?> GetApiKeyByValueAsync(string apiKey, CancellationToken cancellationToken)
    {
        return await context.AppUserApiKeys
            .Where(x => x.Value == apiKey)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<AppUserApiKey?> GetApiKeyByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.AppUserApiKeys
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<AppUserApiKey>> GetApiKeysByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await context.AppUserApiKeys
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}