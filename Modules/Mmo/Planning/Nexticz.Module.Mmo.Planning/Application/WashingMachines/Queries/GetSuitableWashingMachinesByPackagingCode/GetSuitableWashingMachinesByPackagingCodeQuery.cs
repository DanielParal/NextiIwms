using MediatR;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetSuitableWashingMachinesByPackagingCode;

internal record GetSuitableWashingMachinesByPackagingCodeQuery(string PackagingCode, string? SisterPackagingCode) : IRequest<WashingMachine[]>;