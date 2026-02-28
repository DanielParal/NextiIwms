using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Contracts.ApiKeys;

namespace Nexticz.Module.Auth.Application.ApiKeys.Commands.UpdateApiKey;

public record UpdateApiKeyCommand(Guid Id, UpdateApiKeyRequest UpdateApiKeyRequest) : IRequest<ErrorOr<Updated>>;