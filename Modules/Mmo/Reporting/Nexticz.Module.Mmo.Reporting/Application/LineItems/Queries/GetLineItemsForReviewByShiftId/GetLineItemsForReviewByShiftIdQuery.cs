using MediatR;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemsForReviewByShiftId;

internal record GetLineItemsForReviewByShiftIdQuery(Guid ShiftId) : IRequest<LineItemView[]>;