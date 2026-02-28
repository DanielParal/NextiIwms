using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.Partners;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Partners.Commands.CreatePartner;

public class CreatePartnerCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreatePartnerCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreatePartnerCommand command, CancellationToken cancellationToken)
    {
        var createdPartner = new Partner
        {
            Name = command.CreatePartnerRequest.Name,
            Code = command.CreatePartnerRequest.Code,
            Note = command.CreatePartnerRequest.Note,
            Receipt = command.CreatePartnerRequest.Receipt,
            ReceiptCoefficient = command.CreatePartnerRequest.ReceiptCoefficient,
            Dispatch = command.CreatePartnerRequest.Dispatch,
            DispatchCoefficient = command.CreatePartnerRequest.DispatchCoefficient,
            Packaging = command.CreatePartnerRequest.Packaging,
            PackagingCoefficient = command.CreatePartnerRequest.PackagingCoefficient
        };

        unitOfWork.Add(createdPartner);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}