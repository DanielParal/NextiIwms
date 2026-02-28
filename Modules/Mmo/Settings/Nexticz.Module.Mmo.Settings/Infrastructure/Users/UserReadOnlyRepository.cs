using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Users;

internal class UserReadOnlyRepository(ISettingsReadOnlyEventStoreRepository settingsReadOnlyEventStoreRepository) 
    : IUserReadOnlyRepository
{
    public async Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        return await settingsReadOnlyEventStoreRepository
            .GetFirstByConditionAsync<User>(x => x.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase),
                cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetUsersByReceivableNotificationAsync(ReceivableNotification notification, CancellationToken cancellationToken)
    {
        // Note: we cannot use this solution for now: .GetAllByConditionAsync<User>(x => x.ReceivableNotifications.Any(n => n == notification)
        // because marten does not support an array with enum values yet. We would need to store it as a string
        var users = await settingsReadOnlyEventStoreRepository
            .GetAllAsync<User>(cancellationToken);
            
        var selectedUsersWithNotification = users.Where(x => x.ReceivableNotifications.Any(n => n == notification)).ToList();
            
        return selectedUsersWithNotification;
    }
}