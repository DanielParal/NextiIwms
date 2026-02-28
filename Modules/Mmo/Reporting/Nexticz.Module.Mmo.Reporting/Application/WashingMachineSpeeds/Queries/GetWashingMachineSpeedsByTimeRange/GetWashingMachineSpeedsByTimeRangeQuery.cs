using MediatR;
using Nexticz.Module.Mmo.Reporting.Domain.WashingMachineSpeedAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.WashingMachineSpeeds.Queries.GetWashingMachineSpeedsByTimeRange;

internal record GetWashingMachineSpeedsByTimeRangeQuery(DateTimeOffset StartDate, DateTimeOffset EndDate) : IRequest<WashingMachineSpeed[]>;