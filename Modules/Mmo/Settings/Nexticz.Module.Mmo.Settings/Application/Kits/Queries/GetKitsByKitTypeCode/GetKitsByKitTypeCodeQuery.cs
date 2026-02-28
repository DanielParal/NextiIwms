using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsByKitTypeCode;

internal record GetKitsByKitTypeCodeQuery(string KitTypeCode) : IRequest<IReadOnlyList<Kit>>;