using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.Assortments;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Assortments.Commands.CreateAssortment;

public class CreateAssortmentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateAssortmentCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateAssortmentCommand command, CancellationToken cancellationToken)
    {
        var createdAssortment = new Assortment
        {
            Name = command.CreateAssortmentRequest.Name,
            Code = command.CreateAssortmentRequest.Code,
            Receipt = command.CreateAssortmentRequest.Receipt,
            ReceiptCoefficient = command.CreateAssortmentRequest.ReceiptCoefficient,
            Dispatch = command.CreateAssortmentRequest.Dispatch,
            DispatchCoefficient = command.CreateAssortmentRequest.DispatchCoefficient,
            Packaging = command.CreateAssortmentRequest.Packaging,
            PackagingCoefficient = command.CreateAssortmentRequest.PackagingCoefficient
        };
        
        unitOfWork.Add(createdAssortment);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}