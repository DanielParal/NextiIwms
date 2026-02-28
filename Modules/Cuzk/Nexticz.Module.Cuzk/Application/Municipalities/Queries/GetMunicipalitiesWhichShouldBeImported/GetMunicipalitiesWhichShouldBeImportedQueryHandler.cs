using ErrorOr;
using MediatR;
using Nexticz.Module.Cuzk.Application.Municipalities.Queries.GetMunicipalityByCode;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;

namespace Nexticz.Module.Cuzk.Application.Municipalities.Queries.GetMunicipalitiesWhichShouldBeImported;

internal class GetMunicipalitiesWhichShouldBeImportedQueryHandler(ICuzkReadOnlyEventStoreRepository readOnlyRepository)
    : IRequestHandler<GetMunicipalitiesWhichShouldBeImportedQuery, Municipality[]>
{
    public async Task<Municipality[]> Handle(GetMunicipalitiesWhichShouldBeImportedQuery query, CancellationToken cancellationToken)
    {
        var municipalities = await readOnlyRepository.GetAllByConditionAsync<Municipality>(
            x => x.ShouldImportAddressLocation, cancellationToken: cancellationToken);

        return municipalities.ToArray();
    }
}