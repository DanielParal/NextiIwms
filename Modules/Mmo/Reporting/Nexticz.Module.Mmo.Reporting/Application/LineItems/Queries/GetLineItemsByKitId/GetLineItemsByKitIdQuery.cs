using MediatR;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemsByKitId;

internal record GetLineItemsByKitIdQuery(Guid KitId) : IRequest<LineItemView[]>;