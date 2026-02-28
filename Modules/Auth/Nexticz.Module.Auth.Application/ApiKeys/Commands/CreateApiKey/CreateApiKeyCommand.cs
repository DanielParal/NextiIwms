using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Contracts.ApiKeys;

namespace Nexticz.Module.Auth.Application.ApiKeys.Commands.CreateApiKey;

public class CreateApiKeyCommand : IRequest<ErrorOr<Created>>
{
    public required CreateApiKeyRequest CreateApiKeyRequest { get; set; }
}