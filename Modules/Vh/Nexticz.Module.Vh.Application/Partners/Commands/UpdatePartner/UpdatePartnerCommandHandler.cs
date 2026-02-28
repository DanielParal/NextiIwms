using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.Partners;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Partners.Commands.UpdatePartner;

public class UpdatePartnerCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdatePartnerCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdatePartnerCommand command, CancellationToken cancellationToken)
    {
        var partner = await unitOfWork.PartnersRepository.GetPartnerByIdAsync(command.Id, cancellationToken);

        if (partner is null) return PartnersErrors.PartnerWithIdDoesnotExist;

        partner.Name = command.UpdatePartnerRequest.Name;
        partner.Code = command.UpdatePartnerRequest.Code;
        partner.Note = command.UpdatePartnerRequest.Note;
        partner.Receipt = command.UpdatePartnerRequest.Receipt;
        partner.ReceiptCoefficient = command.UpdatePartnerRequest.ReceiptCoefficient;
        partner.Dispatch = command.UpdatePartnerRequest.Dispatch;
        partner.DispatchCoefficient = command.UpdatePartnerRequest.DispatchCoefficient;
        partner.Packaging = command.UpdatePartnerRequest.Packaging;
        partner.PackagingCoefficient = command.UpdatePartnerRequest.PackagingCoefficient;

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Updated;
    }
}