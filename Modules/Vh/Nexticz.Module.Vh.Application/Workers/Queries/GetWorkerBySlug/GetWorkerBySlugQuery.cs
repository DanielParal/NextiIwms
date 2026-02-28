using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Workers;

namespace Nexticz.Module.Vh.Application.Workers.Queries.GetWorkerBySlug;

public class GetWorkerBySlugQuery : IRequest<ErrorOr<WorkerResponse>>
{
    public required string Slug { get; set; }
}