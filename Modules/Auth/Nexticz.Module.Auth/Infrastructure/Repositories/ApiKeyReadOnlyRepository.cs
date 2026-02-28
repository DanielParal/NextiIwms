using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Auth.Application.Interfaces;
using Nexticz.Module.Auth.Domain.ApiKeyAggregate;
using Nexticz.Module.Auth.Infrastructure.Dbs;
using Nexticz.Module.Auth.Infrastructure.Identity;

namespace Nexticz.Module.Auth.Infrastructure.Repositories;

internal class ApiKeyReadOnlyRepository(AuthDataContext dataContext) : IApiKeyReadOnlyRepository
{
    public async Task<ApiKey[]> GetApiKeysByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var appUserApiKeys = 
            await dataContext.AppUserApiKeys
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        var apiKeysToReturn = new List<ApiKey>();
        foreach (var appUserApiKey in appUserApiKeys)
        {
            var mappedApiKey = AppUserApiKeyMapper.ToDomain(appUserApiKey);
            if (mappedApiKey.IsError)
                continue;
            
            apiKeysToReturn.Add(mappedApiKey.Value);
        }
        
        return apiKeysToReturn.ToArray();
    }
}