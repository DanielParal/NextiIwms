using MediatR;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;


namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagings;

internal class GetPackagingsQueryHandler (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository)
    : IRequestHandler<GetPackagingsQuery, FilteredResult<Packaging>>
{
    public async Task<FilteredResult<Packaging>> Handle(GetPackagingsQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<Packaging>(request.FilteringParams, cancellationToken);
    }
}