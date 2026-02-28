using MediatR;
using Nexticz.Module.Portal.Domain.ModuleAggregate;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Portal.Application.Interfaces;

namespace Nexticz.Module.Portal.Application.Modules.Queries.GetModules;

internal class GetModulesQueryHandler(IPortalReadOnlyEventStoreRepository readOnlyRepository) : IRequestHandler<GetModulesQuery, FilteredResult<Domain.ModuleAggregate.Module>>
{
    public async Task<FilteredResult<Domain.ModuleAggregate.Module>> Handle(GetModulesQuery query, CancellationToken cancellationToken)
    {
        return await readOnlyRepository.GetFilteredAsync<Domain.ModuleAggregate.Module>(query.FilteringParams, cancellationToken);
    }
}