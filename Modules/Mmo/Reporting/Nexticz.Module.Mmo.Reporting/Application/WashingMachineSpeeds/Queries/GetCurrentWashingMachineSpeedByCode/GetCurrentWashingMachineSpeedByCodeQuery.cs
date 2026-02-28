using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.WashingMachineSpeeds.Queries.GetCurrentWashingMachineSpeedByCode;

internal record GetCurrentWashingMachineSpeedByCodeQuery(string Code) : IRequest<ErrorOr<WashingMachineSpeed>>;