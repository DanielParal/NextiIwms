using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.ExportEntity;


namespace Nexticz.Module.Mmo.Settings.Application.ImportsExports.Exports.Queries.GetExports;

internal class GetExportsQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository
    ) : IRequestHandler<GetExportsQuery, FilteredResult<Export>>
{
    public async Task<FilteredResult<Export>> Handle(GetExportsQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<Export>(request.FilteringParams, cancellationToken);
    }
}