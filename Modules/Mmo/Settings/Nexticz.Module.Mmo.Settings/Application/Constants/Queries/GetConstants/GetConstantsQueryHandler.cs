using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Constants.Queries.GetConstants;

internal class GetConstantsQueryHandler(ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetConstantsQuery, FilteredResult<Constant>>
{
    public async Task<FilteredResult<Constant>> Handle(GetConstantsQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<Constant>(request.FilteringParams, cancellationToken);
    }
}