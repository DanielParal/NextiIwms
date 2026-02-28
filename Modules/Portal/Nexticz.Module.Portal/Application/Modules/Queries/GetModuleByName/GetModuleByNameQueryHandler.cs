using ErrorOr;
using MediatR;
using Nexticz.Module.Portal.Domain.ModuleAggregate;
using Nexticz.Module.Portal.Application.Interfaces;

namespace Nexticz.Module.Portal.Application.Modules.Queries.GetModuleByName;

internal class GetModuleByNameQueryHandler(IPortalReadOnlyEventStoreRepository readOnlyRepository)
    : IRequestHandler<GetModuleByNameQuery, ErrorOr<Domain.ModuleAggregate.Module>>
{
    public async Task<ErrorOr<Domain.ModuleAggregate.Module>> Handle(GetModuleByNameQuery query, CancellationToken cancellationToken)
    {
        var module = await readOnlyRepository.GetFirstByConditionAsync<Domain.ModuleAggregate.Module>(
            x => string.Equals(x.Name, query.Name, StringComparison.InvariantCultureIgnoreCase), cancellationToken: cancellationToken);

        if (module is null) 
            return ModuleErrors.ValidationModuleWithNameDoesNotExist;

        return module;
    }
}