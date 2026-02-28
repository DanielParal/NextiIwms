using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Partners;
using Nexticz.Module.Vh.Domain.Partners;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Partners.Queries.GetPartnerById;

public class GetPartnerByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetPartnerByIdQuery, ErrorOr<PartnerResponse>>
{
    public async Task<ErrorOr<PartnerResponse>> Handle(GetPartnerByIdQuery query, CancellationToken cancellationToken)
    {
        var partner = await unitOfWork.PartnersRepository.GetPartnerResponseByIdAsync(query.Id, cancellationToken);

        if (partner is null) return PartnersErrors.PartnerWithIdDoesnotExist;

        return partner;
    }
}