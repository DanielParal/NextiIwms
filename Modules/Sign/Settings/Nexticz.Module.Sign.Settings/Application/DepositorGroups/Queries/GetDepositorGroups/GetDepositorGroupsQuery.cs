using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate;


namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.Queries.GetDepositorGroups;

internal record GetDepositorGroupsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<DepositorGroup>>;