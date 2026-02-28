using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Domain.AppUserApiKeys;
using Nexticz.Module.Auth.Application.Common.Interfaces;

namespace Nexticz.Module.Auth.Application.ApiKeys.Commands.UpdateApiKey;

public class UpdateApiKEyCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateApiKeyCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateApiKeyCommand command, CancellationToken cancellationToken)
    {
        var apiKey = await unitOfWork.AppUserApiKeysRepository.GetApiKeyByIdAsync(command.Id, cancellationToken);

        if (apiKey is null)
            return ApiKeyErrors.ApiKeyNotExist;

        apiKey.Description = command.UpdateApiKeyRequest.Description;
        apiKey.Expiration = command.UpdateApiKeyRequest.Expiration;

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Updated;
    }
}