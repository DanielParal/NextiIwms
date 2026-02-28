using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Depositors.Queries.GetDepositors;

internal record GetDepositorsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Depositor>>;