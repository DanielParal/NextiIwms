using MediatR;

namespace Nexticz.Module.Mmo.Reporting.Application.Kits.Notifications;

internal record InactivityCreatedAfterKitFinishedNotification(Guid[] ShiftIds) : INotification;