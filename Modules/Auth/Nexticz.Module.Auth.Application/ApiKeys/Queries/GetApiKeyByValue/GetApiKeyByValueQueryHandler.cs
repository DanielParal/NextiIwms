using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Domain.AppUserApiKeys;
using Nexticz.Module.Auth.Application.Common.Interfaces;

namespace Nexticz.Module.Auth.Application.ApiKeys.Queries.GetApiKeyByValue;

public class GetApiKeyByValueQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetApiKeyByValueQuery, ErrorOr<AppUserApiKey?>>
{
    public async Task<ErrorOr<AppUserApiKey?>> Handle(GetApiKeyByValueQuery query, CancellationToken cancellationToken)
    {
        var apiKey = await unitOfWork.AppUserApiKeysRepository.GetApiKeyByValueAsync(query.Value, cancellationToken);

        if (apiKey is not null)
        {
            apiKey.LastActivity = DateTime.UtcNow;
            await unitOfWork.CompleteAsync(cancellationToken);
        }

        return apiKey;
    }
}