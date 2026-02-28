using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsByManufactureCode;

internal record GetKitsByManufactureCodeQuery(string ManufactureCode) : IRequest<IReadOnlyList<Kit>>;