using ErrorOr;
using MediatR;

namespace Nexticz.Module.Auth.Application.ApiKeys.Commands.DeleteApiKey;

public class DeleteApiKeyCommand : IRequest<ErrorOr<Deleted>>
{
    public required Guid Id { get; set; }
}