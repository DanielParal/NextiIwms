using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;

namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachinesByCodes;

internal record GetWashingMachinesByCodesQuery(string[] Codes) : IRequest<IReadOnlyList<WashingMachine>>;