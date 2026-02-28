using ErrorOr;
using MediatR;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;

namespace Nexticz.Module.Cuzk.Application.Municipalities.Queries.GetMunicipalityByCode;

internal class GetMunicipalityByCodeQueryHandler(ICuzkReadOnlyEventStoreRepository readOnlyRepository)
    : IRequestHandler<GetMunicipalityByCodeQuery, ErrorOr<Municipality>>
{
    public async Task<ErrorOr<Municipality>> Handle(GetMunicipalityByCodeQuery query, CancellationToken cancellationToken)
    {
        var upperCode = query.Code.ToUpperInvariant();
        var municipality = await readOnlyRepository.GetFirstByConditionAsync<Municipality>(
            x => x.Code == upperCode, cancellationToken: cancellationToken);

        if (municipality is null) 
            return MunicipalityErrors.ValidationMunicipalityWithCodeDoesNotExist;

        return municipality;
    }
}