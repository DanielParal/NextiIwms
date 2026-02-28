using MediatR;

namespace Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;

public record GetWashingMachineResponsesQuery() : IRequest<WashingMachineResponse[]>;