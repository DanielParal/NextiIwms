using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Lib.Shared.MessagePublishers;

namespace Nexticz.Module.Auth.Application.MasstransitPublishers;

public interface IAuthPublisher : IBaseMessagePublisher
{
    Task PublishUserChangedMessageAsync(
        Guid userId,
        string username,
        string? firstName, 
        string? lastName,
        string[] roles,
        string[] permissions,
        UserChangeTypeContract userChangeType,
        CancellationToken cancellationToken);
}