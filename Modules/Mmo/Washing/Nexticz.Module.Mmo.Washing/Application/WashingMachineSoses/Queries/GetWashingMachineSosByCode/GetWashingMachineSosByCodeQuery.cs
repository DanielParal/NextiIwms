using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Queries.GetWashingMachineSosByCode;

public record GetWashingMachineSosByCodeQuery(string Code) : IRequest<WashingMachineSos?>;