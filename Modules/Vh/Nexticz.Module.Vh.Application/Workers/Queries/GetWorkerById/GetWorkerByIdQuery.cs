using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Workers;

namespace Nexticz.Module.Vh.Application.Workers.Queries.GetWorkerById;

public class GetWorkerByIdQuery : IRequest<ErrorOr<WorkerResponse>>
{
    public required Guid Id { get; set; }
}