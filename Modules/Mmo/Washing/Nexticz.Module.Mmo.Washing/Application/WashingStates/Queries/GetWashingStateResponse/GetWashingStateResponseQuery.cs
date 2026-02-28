using MediatR;
using Nexticz.Module.Mmo.Washing.Contracts.WashingStates;

namespace Nexticz.Module.Mmo.Washing.Application.WashingStates.Queries.GetWashingStateResponse;

internal record GetWashingStateResponseQuery() : IRequest<WashingStateResponse>;