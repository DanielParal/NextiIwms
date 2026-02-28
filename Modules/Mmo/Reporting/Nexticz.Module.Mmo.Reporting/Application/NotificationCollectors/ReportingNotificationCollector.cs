using MediatR;
using Nexticz.Module.Mmo.SharedKernel.MediatR;

namespace Nexticz.Module.Mmo.Reporting.Application.NotificationCollectors;

internal class ReportingNotificationCollector(IMediator mediator) : NotificationCollector(mediator), IReportingNotificationCollector;