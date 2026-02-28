using Nexticz.Module.Auth.Domain.ApiKeyAggregate;

namespace Nexticz.Module.Auth.Application.Interfaces;

internal interface IApiKeyReadOnlyRepository
{
    Task<ApiKey[]> GetApiKeysByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}