using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Module.Notifications.Application.Interfaces;

namespace Nexticz.Module.Notifications.Infrastructure.BaseRepositories;

internal class NotificationReadOnlyEventStoreRepository(INotificationDocumentSessionProvider documentSessionProvider) 
    : MartenReadOnlyEventStoreRepository(documentSessionProvider), INotificationReadOnlyEventStoreRepository
{
    
}