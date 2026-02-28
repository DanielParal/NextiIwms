using MediatR;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Queries.GetWashingMachineSoses;

internal record GetWashingMachineSosesQuery() : IRequest<WashingMachineSos[]>;