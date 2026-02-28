using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Nexticz.Module.Auth.Domain.AppUserApiKeys;
using Nexticz.Module.Auth.Domain.AppUsers;
using Nexticz.Module.Auth.Application.Common.Interfaces;

namespace Nexticz.Module.Auth.Application.Authentications.Queries.GetAccessTokenForApiKey;

public class GetAccessTokenForApiKeyQueryHandler(
    IUnitOfWork unitOfWork,
    UserManager<AppUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<GetAccessTokenForApiKeyQuery, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(GetAccessTokenForApiKeyQuery query, CancellationToken cancellationToken)
    {
        var apiKey =
            await unitOfWork.AppUserApiKeysRepository.GetApiKeyByValueAsync(query.ApiKey, cancellationToken);

        if (apiKey is null) return ApiKeyErrors.ApiKeyNotExist;

        var user = userManager.Users.FirstOrDefault(x => x.Id == apiKey.UserId);

        if (user is null) return ApiKeyErrors.ApiKeyUserNotExist;

        apiKey.LastActivity = DateTime.UtcNow;

        await unitOfWork.CompleteAsync(cancellationToken);

        return jwtTokenGenerator.GenerateAccessTokenForApiKey(user);
    }
}