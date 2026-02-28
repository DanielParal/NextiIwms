using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Partners.Queries.GetPartnerByCode;

internal class GetPartnerByCodeQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetPartnerByCodeQuery, ErrorOr<Partner>>
{
    public async Task<ErrorOr<Partner>> Handle(GetPartnerByCodeQuery request, CancellationToken cancellationToken)
    {
        var partner = await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<Partner>(
                x => x.Code.Equals(request.Code, StringComparison.InvariantCultureIgnoreCase), 
                cancellationToken);

        if (partner is null)
            return PartnerErrors.CodeDoesNotExist;
        
        return partner;
    }
}