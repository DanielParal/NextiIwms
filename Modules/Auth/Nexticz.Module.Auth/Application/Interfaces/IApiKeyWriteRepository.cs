using ErrorOr;
using Nexticz.Module.Auth.Domain.ApiKeyAggregate;

namespace Nexticz.Module.Auth.Application.Interfaces;

internal interface IApiKeyWriteRepository
{
    Task<ErrorOr<Success>> CreateApiKeyAsync(ApiKey apiKey, CancellationToken cancellationToken);
}