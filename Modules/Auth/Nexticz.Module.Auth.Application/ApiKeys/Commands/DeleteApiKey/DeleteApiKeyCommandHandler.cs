using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Domain.AppUserApiKeys;
using Nexticz.Module.Auth.Application.Common.Interfaces;

namespace Nexticz.Module.Auth.Application.ApiKeys.Commands.DeleteApiKey;

public class DeleteApiKeyCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteApiKeyCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteApiKeyCommand command, CancellationToken cancellationToken)
    {
        var apiKey = await unitOfWork.AppUserApiKeysRepository.GetApiKeyByIdAsync(command.Id, cancellationToken);

        if (apiKey is null) return ApiKeyErrors.ApiKeyNotExist;

        unitOfWork.Remove(apiKey);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Deleted;
    }
}