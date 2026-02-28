using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.Workers.Commands.DeleteWorker;

public class DeleteWorkerCommand : IRequest<ErrorOr<Deleted>>
{
    public required Guid Id { get; set; }
}