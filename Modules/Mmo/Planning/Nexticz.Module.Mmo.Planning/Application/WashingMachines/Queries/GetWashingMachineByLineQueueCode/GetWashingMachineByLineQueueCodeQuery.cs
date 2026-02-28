using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachineByLineQueueCode;

internal record GetWashingMachineByLineQueueCodeQuery(string LineQueueCode) : IRequest<ErrorOr<WashingMachine>>;