using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Drying.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Drying.Application.Kits.Queries.GetKitById;

internal record GetKitByIdQuery(Guid Id) : IRequest<ErrorOr<Kit>>;