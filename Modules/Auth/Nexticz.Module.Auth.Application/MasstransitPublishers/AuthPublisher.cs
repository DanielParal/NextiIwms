using MassTransit;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Lib.Shared.MessagePublishers;

namespace Nexticz.Module.Auth.Application.MasstransitPublishers;

internal class AuthPublisher(IPublishEndpoint publishEndpoint) 
    : BaseMessagePublisher(publishEndpoint), IAuthPublisher
{
    public async Task PublishUserChangedMessageAsync(
        Guid userId, string username, 
        string? firstName, string? lastName, 
        string[] roles, string[] permissions, 
        UserChangeTypeContract userChangeType, CancellationToken cancellationToken)
    {
        var fullName = firstName is null && lastName is null ? 
            null : $"{firstName} {lastName}";
        var message = new UserChangedMessage(userId, username, fullName, roles, permissions, userChangeType);
        await PublishAsync(message, cancellationToken);
    }
}