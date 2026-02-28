using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.LastEnteredWorkerOnLineAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.LastEnteredWorkerOnLines.Queries.GetLastWorkerByLineCode;

internal class GetLastWorkerByLineCodeQueryHandler(
    ILastEnteredWorkerOnLineRepository lastEnteredWorkerOnLineRepository) 
    : IRequestHandler<GetLastWorkerByLineCodeQuery, ErrorOr<LastEnteredWorkerOnLine>>
{
    public async Task<ErrorOr<LastEnteredWorkerOnLine>> Handle(GetLastWorkerByLineCodeQuery request, CancellationToken cancellationToken)
    {
        var lastWorker = await lastEnteredWorkerOnLineRepository.GetLastEnteredWorkerOnLineAsync(request.LineCode, cancellationToken);

        if (lastWorker is null)
            return LastEnteredWorkerOnLineErrors.WorkerNotFound;
        
        return lastWorker;
    }
}