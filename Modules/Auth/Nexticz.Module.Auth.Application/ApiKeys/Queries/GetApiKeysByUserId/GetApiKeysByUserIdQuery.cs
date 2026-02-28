using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Domain.AppUserApiKeys;

namespace Nexticz.Module.Auth.Application.ApiKeys.Queries.GetApiKeyById;

public record GetApiKeysByUserIdQuery(Guid UserId) : IRequest<ErrorOr<List<AppUserApiKey>>>;