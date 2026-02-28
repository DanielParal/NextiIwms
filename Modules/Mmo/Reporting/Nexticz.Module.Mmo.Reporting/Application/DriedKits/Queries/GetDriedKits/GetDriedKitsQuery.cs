using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate;


namespace Nexticz.Module.Mmo.Reporting.Application.DriedKits.Queries.GetDriedKits;

internal record GetDriedKitsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<DriedKit>>;