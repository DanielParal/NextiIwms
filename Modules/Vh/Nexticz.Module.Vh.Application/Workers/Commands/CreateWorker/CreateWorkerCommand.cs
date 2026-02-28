using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Workers;

namespace Nexticz.Module.Vh.Application.Workers.Commands.CreateWorker;

public class CreateWorkerCommand : IRequest<ErrorOr<Created>>
{
    public required CreateWorkerRequest CreateWorkerRequest { get; set; }
}