using ErrorOr;
using MediatR;
using Nexticz.Module.Portal.Domain.ModuleAggregate;
using Nexticz.Module.Portal.Application.Interfaces;

namespace Nexticz.Module.Portal.Application.Modules.Queries.GetModuleById;

internal class GetModuleByIdQueryHandler(IPortalReadOnlyEventStoreRepository readOnlyRepository)
    : IRequestHandler<GetModuleByIdQuery, ErrorOr<Domain.ModuleAggregate.Module>>
{
    public async Task<ErrorOr<Domain.ModuleAggregate.Module>> Handle(GetModuleByIdQuery query, CancellationToken cancellationToken)
    {
        var module = await readOnlyRepository.GetFirstByConditionAsync<Domain.ModuleAggregate.Module>(x => x.Id == query.Id, cancellationToken: cancellationToken);

        if (module is null) 
            return ModuleErrors.ModuleWithIdNotFound;

        return module;
    }
}