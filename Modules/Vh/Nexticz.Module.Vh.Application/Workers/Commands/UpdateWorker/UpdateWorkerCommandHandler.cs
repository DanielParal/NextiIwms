using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.Workers;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Workers.Commands.UpdateWorker;

public class UpdateWorkerCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateWorkerCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateWorkerCommand command, CancellationToken cancellationToken)
    {
        var worker = await unitOfWork.WorkersRepository.GetWorkerByIdAsync(command.Id, cancellationToken);

        if (worker is null) return WorkerErrors.WorkerWithIdDoesnotExist;

        worker.Name = command.UpdateWorkerRequest.Name;
        worker.Surname = command.UpdateWorkerRequest.Surname;
        worker.CodeSag = command.UpdateWorkerRequest.CodeSag;
        worker.CenterId = command.UpdateWorkerRequest.CenterId;
        worker.ActivityAfterCutOffCode = command.UpdateWorkerRequest.ActivityAfterCutOffCode;

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Updated;
    }
}