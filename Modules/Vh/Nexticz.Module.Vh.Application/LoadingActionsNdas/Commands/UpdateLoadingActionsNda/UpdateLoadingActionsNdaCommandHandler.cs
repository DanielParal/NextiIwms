using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.LoadingActionsNdas;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.LoadingActionsNdas.Commands.UpdateLoadingActionsNda;

public class UpdateLoadingActionsNdaCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateLoadingActionsNdaCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateLoadingActionsNdaCommand command, CancellationToken cancellationToken)
    {
        var loadingActionsNda =
            await unitOfWork.LoadingActionsNdasRepository.GetLoadingActionsNdaByIdAsync(command.Id, cancellationToken);

        if (loadingActionsNda is null)
        {
            return LoadingActionsNdaErrors.LoadingActionsNdaWithIdDoesnotExist;
        }

        loadingActionsNda.Created = command.UpdateLoadingActionsNdaRequest.Created;
        loadingActionsNda.Note = command.UpdateLoadingActionsNdaRequest.Note;
        loadingActionsNda.LoadingDeviceId = command.UpdateLoadingActionsNdaRequest.LoadingDeviceId;
        loadingActionsNda.WorkerSlug = command.UpdateLoadingActionsNdaRequest.WorkerSlug;
        loadingActionsNda.NonDispensingActivitySlug = command.UpdateLoadingActionsNdaRequest.NonDispensingActivitySlug;

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Updated;

    }
}