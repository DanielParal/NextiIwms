using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositors;

internal record GetDepositorsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Depositor>>;