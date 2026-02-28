using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.Constants.Queries;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Models;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables;

internal class WashingMachineTimeTableFactory(
    ISender sender,
    IClock clock)
{
    public async Task<WashingMachineTimeTable> CreateAsync(WashingMachine washingMachine, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(washingMachine);

        var adjustmentTimeConstant = await sender.Send(
            new GetWashingMachineAdjustmentTimeConstantValueQuery(), cancellationToken);
        var scheduler = new TimeTableScheduler(TimeSpan.FromMinutes(adjustmentTimeConstant), clock);
        
        var timetable = new WashingMachineTimeTable
        {
            Code = washingMachine.Code,
            Status = washingMachine.Status,
            Queues = scheduler.Schedule(washingMachine.LineQueues)
        };
        
        return timetable;
    }
}