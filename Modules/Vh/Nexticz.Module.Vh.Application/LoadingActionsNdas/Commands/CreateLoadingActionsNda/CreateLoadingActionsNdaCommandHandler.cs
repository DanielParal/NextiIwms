using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.LoadingActionsNdas;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.LoadingActionsNdas.Commands.CreateLoadingActionsNda;

public class CreateLoadingActionsNdaCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateLoadingActionsNdaCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateLoadingActionsNdaCommand command,
        CancellationToken cancellationToken)
    {
        var loadingActionsNda = new LoadingActionsNda
        {
            Created = DateTime.Now.ToUniversalTime(),
            Note = command.CreateLoadingActionsNdaRequest.Note,
            LoadingDeviceId = command.CreateLoadingActionsNdaRequest.LoadingDeviceId,
            WorkerSlug = command.CreateLoadingActionsNdaRequest.WorkerSlug,
            NonDispensingActivitySlug = command.CreateLoadingActionsNdaRequest.NonDispensingActivitySlug
        };

        unitOfWork.Add(loadingActionsNda);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}