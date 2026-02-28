using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.LoadingActionsNdas;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.LoadingActionsNdas.Commands.DeleteLoadingActionsNda;

public class DeleteLoadingActionsNdaCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteLoadingActionsNdaCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteLoadingActionsNdaCommand command,
        CancellationToken cancellationToken)
    {
        var loadingActionsNda =
            await unitOfWork.LoadingActionsNdasRepository.GetLoadingActionsNdaByIdAsync(command.Id, cancellationToken);

        if (loadingActionsNda is null)
        {
            return LoadingActionsNdaErrors.LoadingActionsNdaWithIdDoesnotExist;
        }

        unitOfWork.Remove(loadingActionsNda);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Deleted;

    }
}