using System.Security.Cryptography;
using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Domain.AppUserApiKeys;
using Nexticz.Module.Auth.Application.Common.Interfaces;

namespace Nexticz.Module.Auth.Application.ApiKeys.Commands.CreateApiKey;

public class CreateApiKeyCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateApiKeyCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateApiKeyCommand command, CancellationToken cancellationToken)
    {
        var apiKey = new AppUserApiKey
        {
            Description = command.CreateApiKeyRequest.Description,
            UserId = command.CreateApiKeyRequest.UserId,
            Created = DateTime.UtcNow,
            Expiration = command.CreateApiKeyRequest.Expiration,
            Value = GenerateApiKey()
        };

        unitOfWork.Add(apiKey);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }

    private static string GenerateApiKey()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}