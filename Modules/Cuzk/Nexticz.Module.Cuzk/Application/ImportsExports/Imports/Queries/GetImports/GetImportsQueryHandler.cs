using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Application.ImportsExports.Imports.Queries.GetImports;

internal class GetImportsQueryHandler(ICuzkReadOnlyEventStoreRepository readOnlyRepository) : IRequestHandler<GetImportsQuery, FilteredResult<Import>>
{
    public async Task<FilteredResult<Import>> Handle(GetImportsQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyRepository.GetFilteredAsync<Import>(request.FilteringParams, cancellationToken);
    }
}