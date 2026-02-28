using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.Workers;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Workers.Commands.CreateWorker;

public class CreateWorkerCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateWorkerCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateWorkerCommand command, CancellationToken cancellationToken)
    {
        var createdWorker = new Worker
        {
            Name = command.CreateWorkerRequest.Name,
            Surname = command.CreateWorkerRequest.Surname,
            CodeWms = command.CreateWorkerRequest.CodeWms,
            CodeSag = command.CreateWorkerRequest.CodeSag,
            CenterId = command.CreateWorkerRequest.CenterId,
            ActivityAfterCutOffCode = command.CreateWorkerRequest.ActivityAfterCutOffCode
        };
        
        unitOfWork.Add(createdWorker);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}