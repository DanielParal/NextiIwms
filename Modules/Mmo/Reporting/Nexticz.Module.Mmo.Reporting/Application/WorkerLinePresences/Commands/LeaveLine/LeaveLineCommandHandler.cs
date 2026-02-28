using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.WorkerEntity;
using Nexticz.Module.Mmo.Reporting.Domain.WorkerLinePresenceAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.WorkerLinePresenceAggregate.Events;

namespace Nexticz.Module.Mmo.Reporting.Application.WorkerLinePresences.Commands.LeaveLine;

internal class LeaveLineCommandHandler(
    ILogger<LeaveLineCommandHandler> logger,
    IReportingUnitOfWork unitOfWork)
    : IRequestHandler<LeaveLineCommand, Success>
{
    public Task<Success> Handle(LeaveLineCommand request, CancellationToken cancellationToken)
    {
        var worker = new Worker(request.WorkerName, request.WorkerId);
        var workerLinePresence = new WorkerLinePresence(request.LineCode, worker, null);
        var workerLeftLineEvent = new WorkerLeftLineEvent(
            workerLinePresence.Id,
            request.ShiftId,
            workerLinePresence.LineCode,
            workerLinePresence.Worker?.Id,
            workerLinePresence.Worker?.Name,
            request.LeftAt);
        
        unitOfWork.AppendEvent(workerLinePresence.Id, workerLeftLineEvent);
        
        logger.LogInformation("Reporting - Worker with id {WorkerId} left line {LineCode}.", 
            workerLinePresence.Worker!.Id, workerLinePresence.LineCode);

        return Task.FromResult(Result.Success);
    }
}