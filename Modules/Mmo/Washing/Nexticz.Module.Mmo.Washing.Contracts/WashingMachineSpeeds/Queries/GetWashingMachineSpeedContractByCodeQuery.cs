using MediatR;

namespace Nexticz.Module.Mmo.Washing.Contracts.WashingMachineSpeeds.Queries;

public record GetWashingMachineSpeedContractByCodeQuery(string Code) : IRequest<WashingMachineSpeedContract?>;