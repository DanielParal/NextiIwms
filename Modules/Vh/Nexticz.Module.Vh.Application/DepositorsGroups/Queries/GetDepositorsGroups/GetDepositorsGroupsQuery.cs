using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.DepositorsGroups.Common.Models;


namespace Nexticz.Module.Vh.Application.DepositorsGroups.Queries.GetDepositorsGroups;

public class GetDepositorsGroupsQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required DepositorsGroupsFilteringParams FilteringParams { get; set; }
}