using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;


namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKits;

internal record GetKitsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Kit>>;