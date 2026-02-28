using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Auth.Application.Interfaces;
using Nexticz.Module.Auth.Domain.ApiKeyAggregate;
using Nexticz.Module.Auth.Infrastructure.Identity;

namespace Nexticz.Module.Auth.Infrastructure.Repositories;

internal class ApiKeyWriteRepository(
    ILogger<ApiKeyWriteRepository> logger,
    IAuthUnitOfWork authUnitOfWork) : IApiKeyWriteRepository
{
    public async Task<ErrorOr<Success>> CreateApiKeyAsync(ApiKey apiKey, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Auth] [Start] [CreateApiKeyAsync] apiKeyId: {Id}, userId: {UserId}, apiKeyDescription: {Description}", 
            apiKey.Id, apiKey.UserId, apiKey.Description);
        
        var dbApiKey = new AppUserApiKey
        {
            Id = apiKey.Id,
            Description = apiKey.Description,
            UserId = apiKey.UserId,
            Created = apiKey.Created.UtcDateTime,
            Expiration = apiKey.Expiration?.UtcDateTime,
            Value = apiKey.Value
        };

        authUnitOfWork.Add(dbApiKey);

        await authUnitOfWork.CompleteAsync(cancellationToken);

        logger.LogInformation("[Auth] [End] [CreateApiKeyAsync] apiKey created in db. Id: {Id}", dbApiKey.Id);

        return Result.Success;
    }
}