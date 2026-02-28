using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Drying.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Drying.Application.Kits.Queries.GetKitByCompletedKitsCount;

internal record GetKitByCompletedKitsCountQuery(int CompletedKitsCount) : IRequest<ErrorOr<Kit>>;