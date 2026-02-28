using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Workers.Queries;

namespace Nexticz.Module.Mmo.Washing.Application.Simulations;

internal class SimulationValidator(ISender sender, ILogger<SimulationOnDemandBackgroundService> logger)
{
    public async Task<ErrorOr<Success>> VerifyWorkersExistAsync(CancellationToken cancellationToken)
    {
        var worker1 = await sender.Send(new GetWorkerResponseByPinQuery(SimulationOnDemandBackgroundService.HardCodedWorker1Pin), cancellationToken);
        if (worker1.IsError)
        {
            logger.LogWarning("Washing - {WorkerValidator} Worker with pin {WorkerPin} does not exist. We need this user in order to simulate the washing process.", 
                nameof(SimulationValidator), SimulationOnDemandBackgroundService.HardCodedWorker1Pin);
            return SimulationErrors.ValidationWorkersForSimulationDoesNotExist(
                SimulationOnDemandBackgroundService.HardCodedWorker1Pin, SimulationOnDemandBackgroundService.HardCodedWorker2Pin);
        }
        
        var worker2 = await sender.Send(new GetWorkerResponseByPinQuery(SimulationOnDemandBackgroundService.HardCodedWorker2Pin), cancellationToken);
        if (worker2.IsError)
        {
            logger.LogWarning("Washing - {WorkerValidator} Worker with pin {WorkerPin} does not exist. We need this user in order to simulate the washing process.", 
                nameof(SimulationValidator), SimulationOnDemandBackgroundService.HardCodedWorker1Pin);
            return SimulationErrors.ValidationWorkersForSimulationDoesNotExist(
                SimulationOnDemandBackgroundService.HardCodedWorker1Pin, SimulationOnDemandBackgroundService.HardCodedWorker2Pin);
        }
        
        return Result.Success;
    }
}