using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;


namespace Nexticz.Module.Mmo.Settings.Application.ImportsExports.Imports.Queries.GetImports;

internal class GetImportsQueryHandler(ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetImportsQuery, FilteredResult<Import>>
{
    public async Task<FilteredResult<Import>> Handle(GetImportsQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<Import>(request.FilteringParams, cancellationToken);
    }
}