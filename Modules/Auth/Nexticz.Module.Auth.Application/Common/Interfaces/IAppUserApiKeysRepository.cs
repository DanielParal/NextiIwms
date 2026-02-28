using Nexticz.Module.Auth.Domain.AppUserApiKeys;

namespace Nexticz.Module.Auth.Application.Common.Interfaces;

public interface IAppUserApiKeysRepository
{
    Task<AppUserApiKey?> GetApiKeyByValueAsync(string apiKey, CancellationToken cancellationToken);
    Task<AppUserApiKey?> GetApiKeyByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<AppUserApiKey>> GetApiKeysByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}