using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.Partners;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Partners.Commands.DeletePartner;

public class DeletePartnerCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeletePartnerCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeletePartnerCommand command, CancellationToken cancellationToken)
    {
        var partner = await unitOfWork.PartnersRepository.GetPartnerByIdAsync(command.Id, cancellationToken);

        if (partner is null) return PartnersErrors.PartnerWithIdDoesnotExist;

        unitOfWork.Remove(partner);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Deleted;
    }
}