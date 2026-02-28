using MediatR;
using Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Models;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Queries.GetSuitableWashingMachinesTimeTablesByPackagingCode;

internal record GetSuitableWashingMachinesTimeTablesByPackagingCodeQuery(string PackagingCode, string? SisterPackagingCode) : IRequest<WashingMachineTimeTable[]>;