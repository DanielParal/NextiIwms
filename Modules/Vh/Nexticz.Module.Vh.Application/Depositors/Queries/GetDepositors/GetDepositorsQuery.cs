using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Depositors.Common.Models;


namespace Nexticz.Module.Vh.Application.Depositors.Queries.GetDepositors;

public class GetDepositorsQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required DepositorsFilteringParams FilteringParams { get; set; }
}