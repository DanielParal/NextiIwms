using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;


namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachines;

internal record GetWashingMachinesQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<WashingMachine>>;