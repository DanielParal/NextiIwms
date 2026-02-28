using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetFilteredWashingMachineResponsesByPackagingCode;

internal record GetFilteredWashingMachineResponsesByPackagingCodeQuery(string PackagingCode, string? SisterPackagingCode) : IRequest<WashingMachineResponse[]>;