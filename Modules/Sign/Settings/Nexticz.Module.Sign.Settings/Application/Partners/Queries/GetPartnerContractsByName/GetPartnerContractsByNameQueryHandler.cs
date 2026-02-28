using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.Partners;
using Nexticz.Module.Sign.Settings.Contracts.Partners.Queries;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Partners.Queries.GetPartnerContractsByName;

internal class GetPartnerContractsByNameQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IRequestHandler<GetPartnerContractsByNameQuery, PartnerContract[]>
{
    public async Task<PartnerContract[]> Handle(GetPartnerContractsByNameQuery request, CancellationToken cancellationToken)
    {
        var partners = 
            await readOnlyEventStoreRepository.GetAllByConditionAsync<Partner>(
                x => x.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);
        
        return partners.Select(PartnerContractFactory.Create).ToArray();
    }
}