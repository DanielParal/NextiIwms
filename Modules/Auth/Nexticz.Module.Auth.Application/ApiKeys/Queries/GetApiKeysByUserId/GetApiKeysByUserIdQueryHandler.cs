using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Domain.AppUserApiKeys;
using Nexticz.Module.Auth.Application.ApiKeys.Queries.GetApiKeyById;
using Nexticz.Module.Auth.Application.Common.Interfaces;

namespace Nexticz.Module.Auth.Application.ApiKeys.Queries.GetApiKeysByUserId;

public class GetApiKeysByUserIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetApiKeysByUserIdQuery, ErrorOr<List<AppUserApiKey>>>
{
    public async Task<ErrorOr<List<AppUserApiKey>>> Handle(GetApiKeysByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        return await unitOfWork.AppUserApiKeysRepository.GetApiKeysByUserIdAsync(query.UserId, cancellationToken);
    }
}