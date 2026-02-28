using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.Assortments;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Assortments.Commands.UpdateAssortment;

public class UpdateAssortmentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateAssortmentCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateAssortmentCommand command, CancellationToken cancellationToken)
    {
        var assortment = await unitOfWork.AssortmentRepository.GetAssortmentByIdAsync(command.Id, cancellationToken);

        if (assortment is null) return AssortmentErrors.AssortmentWithIdDoesnotExist;

        assortment.Name = command.UpdateAssortmentRequest.Name;
        assortment.Code = command.UpdateAssortmentRequest.Code;
        assortment.Receipt = command.UpdateAssortmentRequest.Receipt;
        assortment.ReceiptCoefficient = command.UpdateAssortmentRequest.ReceiptCoefficient;
        assortment.Dispatch = command.UpdateAssortmentRequest.Dispatch;
        assortment.DispatchCoefficient = command.UpdateAssortmentRequest.DispatchCoefficient;
        assortment.Packaging = command.UpdateAssortmentRequest.Packaging;
        assortment.PackagingCoefficient = command.UpdateAssortmentRequest.PackagingCoefficient;

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Updated;
    }
}