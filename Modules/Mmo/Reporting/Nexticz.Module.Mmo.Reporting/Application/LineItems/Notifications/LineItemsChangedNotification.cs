using MediatR;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Notifications;

internal record LineItemsChangedNotification(Guid ShiftId) : INotification;