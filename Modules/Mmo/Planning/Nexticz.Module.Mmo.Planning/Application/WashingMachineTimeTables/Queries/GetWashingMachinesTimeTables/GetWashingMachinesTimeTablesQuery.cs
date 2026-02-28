using MediatR;
using Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Models;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Queries.GetWashingMachinesTimeTables;

internal record GetWashingMachinesTimeTablesQuery() : IRequest<WashingMachineTimeTable[]>;