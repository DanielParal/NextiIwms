using MediatR;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemsByTimeRange;

internal record GetLineItemsByTimeRangeQuery(DateTimeOffset StartDate, DateTimeOffset EndDate) : IRequest<LineItemView[]>;