using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.Workers;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Workers.Commands.DeleteWorker;

public class DeleteWorkerCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteWorkerCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteWorkerCommand command, CancellationToken cancellationToken)
    {
        var worker = await unitOfWork.WorkersRepository.GetWorkerByIdAsync(command.Id, cancellationToken);

        if (worker is null) return WorkerErrors.WorkerWithIdDoesnotExist;

        unitOfWork.Remove(worker);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Deleted;
    }
}