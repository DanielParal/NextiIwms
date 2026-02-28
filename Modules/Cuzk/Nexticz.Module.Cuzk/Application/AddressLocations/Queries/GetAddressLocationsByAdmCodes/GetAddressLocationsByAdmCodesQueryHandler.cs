using Marten;
using MediatR;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

namespace Nexticz.Module.Cuzk.Application.AddressLocations.Queries.GetAddressLocationsByAdmCodes;

internal class GetAddressLocationsByAdmCodesQueryHandler(ICuzkReadOnlyEventStoreRepository readOnlyRepository)
    : IRequestHandler<GetAddressLocationsByAdmCodesQuery, AddressLocation[]>
{
    public async Task<AddressLocation[]> Handle(GetAddressLocationsByAdmCodesQuery query, CancellationToken cancellationToken)
    {
        var upperAdmCodes = query.AdmCodes.Select(x => x.ToUpperInvariant()).ToArray();;
        var addressLocations = await readOnlyRepository.GetAllByConditionAsync<AddressLocation>(
            x => x.AdmCode.IsOneOf(upperAdmCodes), cancellationToken: cancellationToken);
        
        return addressLocations.ToArray();
    }
}