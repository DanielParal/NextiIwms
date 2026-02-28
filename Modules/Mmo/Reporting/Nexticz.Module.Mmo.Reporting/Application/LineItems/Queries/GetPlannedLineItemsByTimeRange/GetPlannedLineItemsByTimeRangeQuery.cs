
using MediatR;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetPlannedLineItemsByTimeRange;

internal record GetPlannedLineItemsByTimeRangeQuery(string LineCode, DateTimeOffset StartDate, DateTimeOffset EndDate) : IRequest<LineItemView[]>;