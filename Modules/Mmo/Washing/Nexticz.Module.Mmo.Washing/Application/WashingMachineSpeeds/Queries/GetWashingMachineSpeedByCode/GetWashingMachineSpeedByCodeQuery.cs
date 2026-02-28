using MediatR;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSpeeds.Queries.GetWashingMachineSpeedByCode;

internal record GetWashingMachineSpeedByCodeQuery(string Code) : IRequest<WashingMachineSpeed?>;