using MediatR;
using ErrorOr;
using Nexticz.Module.Sign.Settings.Contracts.Partners;
using Nexticz.Module.Sign.Settings.Application.Partners.Queries.GetPartnerByCode;
using Nexticz.Module.Sign.Settings.Contracts.Partners.Queries;

namespace Nexticz.Module.Sign.Settings.Application.Partners.Queries.GetPartnerContractByCode;

internal class GetPartnerContractByCodeQueryHandler(ISender sender) : IRequestHandler<GetPartnerContractByCodeQuery, ErrorOr<PartnerContract>>
{
    public async Task<ErrorOr<PartnerContract>> Handle(GetPartnerContractByCodeQuery request, CancellationToken cancellationToken)
    {
        var partner = await sender.Send(new GetPartnerByCodeQuery(request.Code), cancellationToken);
        
        if (partner.IsError)
            return partner.Errors;
        
        return PartnerContractFactory.Create(partner.Value);
    }
}