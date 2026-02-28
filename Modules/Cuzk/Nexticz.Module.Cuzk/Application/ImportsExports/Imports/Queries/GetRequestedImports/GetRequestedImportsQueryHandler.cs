using MediatR;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Application.ImportsExports.Imports.Queries.GetRequestedImports;

internal class GetRequestedImportsQueryHandler(ICuzkReadOnlyEventStoreRepository readOnlyRepository) : IRequestHandler<GetRequestedImportsQuery, Import[]>
{
    public async Task<Import[]> Handle(GetRequestedImportsQuery request, CancellationToken cancellationToken)
    {
        var imports = await readOnlyRepository.GetAllByConditionAsync<Import>(
            x => x.Status == ImportStatus.Requested, cancellationToken);
        
        return imports.ToArray();
    }
}