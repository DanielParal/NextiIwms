using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Module.Mmo.Washing.Domain.KitWashCycleEntity;

namespace Nexticz.Module.Mmo.Washing.Application.Batches;

internal static class KitWashCycleContractFactory
{
    public static KitWashCycleContract Create(KitWashCycle kitWashCycle)
    {
        return new KitWashCycleContract(
                kitWashCycle.Id, kitWashCycle.StartDate,
                kitWashCycle.Efficiency, kitWashCycle.EndDate, kitWashCycle.WorkerName, kitWashCycle.GlobalKitsCount);
    }
}