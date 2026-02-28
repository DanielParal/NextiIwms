using MediatR;

namespace Nexticz.Module.Mmo.Settings.Contracts.Constants.Queries;

public record GetWashingMachineAdjustmentTimeConstantValueQuery() : IRequest<int>;