using MediatR;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Application.AddressLocations.Queries.GetAddressLocationsByAdmCodes;
using Nexticz.Module.Cuzk.Application.ElasticSearch;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

namespace Nexticz.Module.Cuzk.Application.AddressLocations.Queries.GetAddressLocationsBySlug;

internal class GetAddressLocationsBySlugQueryHandler(
    ISender sender,
    IAddressLocationSearch addressLocationSearch) 
    : IRequestHandler<GetAddressLocationsBySlugQuery, AddressLocation[]>
{
    public async Task<AddressLocation[]> Handle(GetAddressLocationsBySlugQuery request, CancellationToken cancellationToken)
    {
        var take = request.Take ?? 10;
        var searchedTerm = await addressLocationSearch.SearchAsync(request.Slug, take, cancellationToken);
        
        var addressLocations = await sender.Send(
            new GetAddressLocationsByAdmCodesQuery(
                searchedTerm.Select(x => x.AdmCode).ToArray()), 
            cancellationToken);
        
        return addressLocations.ToArray();
    }
}