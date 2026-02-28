using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemById;

internal record GetLineItemByIdQuery(Guid Id) : IRequest<ErrorOr<LineItemView>>;