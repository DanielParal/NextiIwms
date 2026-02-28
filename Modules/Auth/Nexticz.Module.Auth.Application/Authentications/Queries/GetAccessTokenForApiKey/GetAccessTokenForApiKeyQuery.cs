using ErrorOr;
using MediatR;

namespace Nexticz.Module.Auth.Application.Authentications.Queries.GetAccessTokenForApiKey;

public record GetAccessTokenForApiKeyQuery(string ApiKey) : IRequest<ErrorOr<string>>;