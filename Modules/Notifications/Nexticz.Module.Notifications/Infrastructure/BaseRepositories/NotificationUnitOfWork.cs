using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Notifications.Application.Interfaces;

namespace Nexticz.Module.Notifications.Infrastructure.BaseRepositories;

internal class NotificationUnitOfWork(INotificationDocumentSessionProvider documentSessionProvider, ICurrentUserProvider currentUserProvider)
    : MartenUnitOfWork(documentSessionProvider, currentUserProvider), INotificationUnitOfWork;