using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachineByCode;

internal record GetWashingMachineByCodeQuery(string Code) : IRequest<ErrorOr<WashingMachine>>;