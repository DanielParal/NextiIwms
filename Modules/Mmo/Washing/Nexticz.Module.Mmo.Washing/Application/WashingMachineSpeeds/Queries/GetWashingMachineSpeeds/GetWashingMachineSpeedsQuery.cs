using MediatR;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSpeeds.Queries.GetWashingMachineSpeeds;

internal record GetWashingMachineSpeedsQuery() : IRequest<WashingMachineSpeed[]>;