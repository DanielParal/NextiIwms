using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;

namespace Nexticz.Module.Cuzk.Application.Municipalities.Queries.GetMunicipalities;

internal class GetMunicipalitiesQueryHandler(ICuzkReadOnlyEventStoreRepository readOnlyRepository) : IRequestHandler<GetMunicipalitiesQuery, FilteredResult<Municipality>>
{
    public async Task<FilteredResult<Municipality>> Handle(GetMunicipalitiesQuery query, CancellationToken cancellationToken)
    {
        return await readOnlyRepository.GetFilteredAsync<Municipality>(query.FilteringParams, cancellationToken);
    }
}