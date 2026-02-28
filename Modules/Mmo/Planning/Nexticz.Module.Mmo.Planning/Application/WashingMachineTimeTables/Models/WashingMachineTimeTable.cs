using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Models;

internal class WashingMachineTimeTable
{
    public string Code { get; set; }
    public WashingMachineStatus Status { get; set; }
    public List<TimeTableQueue> Queues { get; set; }
}