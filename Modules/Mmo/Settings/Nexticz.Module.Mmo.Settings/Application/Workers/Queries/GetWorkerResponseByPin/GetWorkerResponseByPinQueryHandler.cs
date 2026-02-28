using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Contracts.Workers;
using Nexticz.Module.Mmo.Settings.Contracts.Workers.Queries;
using Nexticz.Module.Mmo.Settings.Application.Workers.Queries.GetWorkerByPin;

namespace Nexticz.Module.Mmo.Settings.Application.Workers.Queries.GetWorkerResponseByPin;

internal class GetWorkerResponseByPinQueryHandler(ISender sender) 
    : IRequestHandler<GetWorkerResponseByPinQuery, ErrorOr<WorkerResponse>>
{
    public async Task<ErrorOr<WorkerResponse>> Handle(GetWorkerResponseByPinQuery request, CancellationToken cancellationToken)
    {
        var workerResponse = await sender.Send(new GetWorkerByPinQuery(request.Pin), cancellationToken);
        
        if (workerResponse.IsError)
            return workerResponse.Errors;
        
        return WorkerResponseFactory.Create(workerResponse.Value);
    }
}