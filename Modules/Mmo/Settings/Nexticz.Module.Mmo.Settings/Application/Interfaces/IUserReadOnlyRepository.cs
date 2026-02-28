using Nexticz.Module.Mmo.Settings.Domain.UserEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Interfaces;

internal interface IUserReadOnlyRepository
{
    Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken);
    Task<IReadOnlyList<User>> GetUsersByReceivableNotificationAsync(ReceivableNotification notification, CancellationToken cancellationToken);
}