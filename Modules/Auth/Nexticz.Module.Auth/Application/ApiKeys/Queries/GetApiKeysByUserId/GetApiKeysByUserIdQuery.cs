using MediatR;
using Nexticz.Module.Auth.Application.Interfaces;
using Nexticz.Module.Auth.Domain.ApiKeyAggregate;

namespace Nexticz.Module.Auth.Application.ApiKeys.Queries.GetApiKeysByUserId;

internal record GetApiKeysByUserIdQuery(Guid UserId) : IRequest<ApiKey[]>;

internal class GetApiKeysByUserIdQueryHandler(
    IApiKeyReadOnlyRepository apiKeyReadOnlyRepository) : IRequestHandler<GetApiKeysByUserIdQuery, ApiKey[]>
{
    public async Task<ApiKey[]> Handle(GetApiKeysByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await apiKeyReadOnlyRepository.GetApiKeysByUserIdAsync(request.UserId, cancellationToken);
    }
}