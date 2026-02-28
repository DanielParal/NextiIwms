using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Auth.Application.Interfaces;
using Nexticz.Module.Auth.Contracts.ApiKeys;
using Nexticz.Module.Auth.Domain.ApiKeyAggregate;

namespace Nexticz.Module.Auth.Application.ApiKeys.Commands.CreateApiKey;

internal record CreateApiKeyCommand(CreateApiKeyRequest Request) : IRequest<ErrorOr<ApiKey>>;

internal class CreateApiKeyCommandHandler(
    IClock clock,
    ILogger<CreateApiKeyCommandHandler> logger,
    IUserReadOnlyRepository userReadOnlyRepository,
    IApiKeyWriteRepository apiKeyWriteRepository) : IRequestHandler<CreateApiKeyCommand, ErrorOr<ApiKey>>
{
    public async Task<ErrorOr<ApiKey>> Handle(CreateApiKeyCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Auth] [Start] [CreateApiKeyCommandHandler] request: {@Request}", request.Request);
        
        var user = await userReadOnlyRepository.GetUserByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("[Auth] [End] [CreateApiKeyCommandHandler] user with id does not exist. UserId: {UserId}", request.Request.UserId);
            return ApiKeyErrors.ValidationUserWithIdDoesNotExist;
        }

        var apiKey = ApiKey.CreateFrom(request.Request.UserId, request.Request.Description, clock.UtcNowOffset, null, request.Request.Expiration);
        
        if (apiKey.IsError)
        {
            logger.LogWarning("[Auth] [End] [CreateApiKeyCommandHandler] error creating apikey. Request: {@Request}", request.Request);
            return apiKey.Errors;
        }
        
        var createResult = await apiKeyWriteRepository.CreateApiKeyAsync(apiKey.Value, cancellationToken);
        
        if (createResult.IsError)
        {
            logger.LogWarning("[Auth] [End] [CreateApiKeyCommandHandler] error creating api key in the db. ApiKeyId: {ApiKeyId}, UserId: {UserId}, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}", 
                apiKey.Value.Id, apiKey.Value.UserId, createResult.FirstError.Code, createResult.FirstError.Description);
            return createResult.Errors;
        }
        
        logger.LogInformation("[Auth] [End] [CreateApiKeyCommandHandler] apikey created. ApiKeyId: {ApiKeyId}", apiKey.Value.Id);
        return apiKey.Value;
    }
}