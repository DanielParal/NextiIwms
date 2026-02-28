using MediatR;
using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;

public record GetWashingMachineResponseByCodeQuery(string Code) : IRequest<ErrorOr<WashingMachineResponse>>;