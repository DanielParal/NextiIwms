using ErrorOr;
using MediatR;
using Nexticz.Module.Cuzk.Application.AddressLocations;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

namespace Nexticz.Module.Cuzk.Application.AddressLocations.Queries.GetAddressLocationByAdmCode;

internal class GetAddressLocationByAdmCodeQueryHandler(ICuzkReadOnlyEventStoreRepository readOnlyRepository)
    : IRequestHandler<GetAddressLocationByAdmCodeQuery, ErrorOr<AddressLocation>>
{
    public async Task<ErrorOr<AddressLocation>> Handle(GetAddressLocationByAdmCodeQuery query, CancellationToken cancellationToken)
    {
        var upperAdmCode = query.AdmCode.ToUpperInvariant();
        var addressLocation = await readOnlyRepository.GetFirstByConditionAsync<AddressLocation>(
            x => x.AdmCode == upperAdmCode, cancellationToken: cancellationToken);
        
        if (addressLocation is null)
            return AddressLocationErrors.ValidationAddressLocationWithAdmCodeDoesNotExist;
        
        return addressLocation;
    }
}