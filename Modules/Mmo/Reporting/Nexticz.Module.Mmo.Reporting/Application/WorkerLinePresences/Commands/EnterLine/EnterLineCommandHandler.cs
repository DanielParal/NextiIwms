using ErrorOr;
using Marten.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.WorkerEntity;
using Nexticz.Module.Mmo.Reporting.Domain.WorkerLinePresenceAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.WorkerLinePresenceAggregate.Events;

namespace Nexticz.Module.Mmo.Reporting.Application.WorkerLinePresences.Commands.EnterLine;

internal class EnterLineCommandHandler(
    IReportingUnitOfWork unitOfWork,
    ILogger<EnterLineCommandHandler> logger) 
    : IRequestHandler<EnterLineCommand, Success>
{
    public Task<Success> Handle(EnterLineCommand request, CancellationToken cancellationToken)
    {
        var worker = new Worker(request.WorkerName, request.WorkerId);
        var workerLinePresence = new WorkerLinePresence(request.LineCode, worker, request.EnteredAt);
        var workerEnteredLineEvent = new WorkerEnteredLineEvent(
            workerLinePresence.Id,
            request.ShiftId,
            workerLinePresence.LineCode,
            worker.Id,
            worker.Name,
            request.EnteredAt);
        
        try
        {
            unitOfWork.AppendEvent(workerLinePresence.Id, workerEnteredLineEvent);
        }
        catch (NonExistentStreamException)
        {
            unitOfWork.StartStream<WorkerEnteredLineEvent, WorkerLinePresence>(workerLinePresence.Id, workerEnteredLineEvent);
        }
        
        logger.LogInformation("Reporting - Worker with id {WorkerId} entered line {LineCode}.", 
            workerLinePresence.Worker!.Id, workerLinePresence.LineCode);
        
        return Task.FromResult(Result.Success);
    }
}