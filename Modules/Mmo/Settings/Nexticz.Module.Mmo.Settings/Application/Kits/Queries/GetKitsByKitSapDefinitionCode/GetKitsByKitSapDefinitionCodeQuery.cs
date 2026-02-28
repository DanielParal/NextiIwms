using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsByKitSapDefinitionCode;

internal record GetKitsByKitSapDefinitionCodeQuery(string KitSapDefinitionCode) : IRequest<IReadOnlyList<Kit>>;