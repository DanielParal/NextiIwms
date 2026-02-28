using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Workers;

namespace Nexticz.Module.Vh.Application.Workers.Commands.UpdateWorker;

public class UpdateWorkerCommand : IRequest<ErrorOr<Updated>>
{
    public required Guid Id { get; set; }
    public required UpdateWorkerRequest UpdateWorkerRequest { get; set; }
}