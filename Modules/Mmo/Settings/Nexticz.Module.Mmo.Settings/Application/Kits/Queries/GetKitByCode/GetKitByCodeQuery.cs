using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitByCode;

internal record GetKitByCodeQuery(string Code) : IRequest<ErrorOr<Kit>>;