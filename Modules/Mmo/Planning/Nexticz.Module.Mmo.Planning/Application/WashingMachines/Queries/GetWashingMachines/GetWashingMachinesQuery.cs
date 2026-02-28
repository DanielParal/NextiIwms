using MediatR;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachines;

internal record GetWashingMachinesQuery() : IRequest<WashingMachine[]>;