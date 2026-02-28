using MediatR;
using Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Notifications;

internal record ItemsAddedNotification(Guid ShiftId, InactivityType Type) : INotification;