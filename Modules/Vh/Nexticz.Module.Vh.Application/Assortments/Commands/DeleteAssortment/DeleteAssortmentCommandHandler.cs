using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.Assortments;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Assortments.Commands.DeleteAssortment;

public class DeleteAssortmentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteAssortmentCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteAssortmentCommand command, CancellationToken cancellationToken)
    {
        var assortment = await unitOfWork.AssortmentRepository.GetAssortmentByIdAsync(command.Id, cancellationToken);

        if (assortment is null) return AssortmentErrors.AssortmentWithIdDoesnotExist;
        
        unitOfWork.Remove(assortment);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Deleted;
    }
}