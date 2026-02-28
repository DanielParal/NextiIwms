using MediatR;
using ErrorOr;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.Queries.GetDepositorGroupByCode;

internal class GetDepositorGroupByCodeQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetDepositorGroupByCodeQuery, ErrorOr<DepositorGroup>>
{
    public async Task<ErrorOr<DepositorGroup>> Handle(GetDepositorGroupByCodeQuery request, CancellationToken cancellationToken)
    {
        var depositorGroup = await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<DepositorGroup>(
                x => x.Code.Equals(request.Code, StringComparison.InvariantCultureIgnoreCase), 
                cancellationToken);

        if (depositorGroup is null)
            return DepositorGroupErrors.CodeDoesNotExist;
        
        return depositorGroup;
    }
}