using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Drying.Domain.KitAggregate;


namespace Nexticz.Module.Mmo.Drying.Application.Kits.Queries.GetKits;

internal record GetKitsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Kit>>;