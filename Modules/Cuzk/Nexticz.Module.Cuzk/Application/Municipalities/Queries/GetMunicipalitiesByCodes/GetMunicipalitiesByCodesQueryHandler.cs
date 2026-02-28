using Marten;
using MediatR;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;

namespace Nexticz.Module.Cuzk.Application.Municipalities.Queries.GetMunicipalitiesByCodes;

internal class GetMunicipalitiesByCodesQueryHandler(ICuzkReadOnlyEventStoreRepository readOnlyRepository)
    : IRequestHandler<GetMunicipalitiesByCodesQuery, Municipality[]>
{
    public async Task<Municipality[]> Handle(GetMunicipalitiesByCodesQuery query, CancellationToken cancellationToken)
    {
        var upperCodes = query.Codes.Select(x => x.ToUpperInvariant()).ToArray();;
        var addressLocations = await readOnlyRepository.GetAllByConditionAsync<Municipality>(
            x => x.Code.IsOneOf(upperCodes), cancellationToken: cancellationToken);
        
        return addressLocations.ToArray();
    }
}