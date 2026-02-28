using MediatR;

namespace Nexticz.Module.Mmo.Washing.Contracts.WashingMachineSoses.Queries;

public record GetWashingMachineSosResponsesQuery() : IRequest<WashingMachineSosResponse[]>;