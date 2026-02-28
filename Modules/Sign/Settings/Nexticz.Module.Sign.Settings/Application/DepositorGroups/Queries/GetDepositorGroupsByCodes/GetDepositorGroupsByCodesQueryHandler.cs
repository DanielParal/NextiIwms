using Marten;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.Queries.GetDepositorGroupsByCodes;

internal class GetDepositorGroupsByCodesQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetDepositorGroupsByCodesQuery, DepositorGroup[]>
{
    public async Task<DepositorGroup[]> Handle(GetDepositorGroupsByCodesQuery request, CancellationToken cancellationToken)
    {
        var upperCodes = request.Codes.Select(x => x.ToUpperInvariant()).ToArray();
        var depositorGroups = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<DepositorGroup>(
                x => x.Code.IsOneOf(upperCodes), 
                cancellationToken);

        return depositorGroups.ToArray();
    }
}