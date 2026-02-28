using MediatR;

namespace Nexticz.Module.Mmo.Settings.Contracts.Constants.Queries;

public record GetSpaceBetweenPackagingsOnWashingMachineConstantValueQuery() : IRequest<int>;