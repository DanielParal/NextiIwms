using MediatR;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Notifications;

internal record CommentChangedNotification(Guid LineItemId, Guid ShiftId, string Comment) : INotification;