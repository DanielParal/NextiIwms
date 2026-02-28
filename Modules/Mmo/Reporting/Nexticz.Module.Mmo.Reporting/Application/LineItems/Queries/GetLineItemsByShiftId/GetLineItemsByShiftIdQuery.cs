using MediatR;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemsByShiftId;

internal record GetLineItemsByShiftIdQuery(Guid ShiftId) : IRequest<LineItemView[]>;