using MediatR;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Queries.GetWashingMachineSoses;

internal class GetWashingMachineSosesQueryHandler(
    IWashingReadOnlyEventStoreRepository washingReadOnlyEventStoreRepository) : IRequestHandler<GetWashingMachineSosesQuery, WashingMachineSos[]>
{
    public async Task<WashingMachineSos[]> Handle(GetWashingMachineSosesQuery request, CancellationToken cancellationToken)
    {
        var soses = await washingReadOnlyEventStoreRepository.GetAllAsync<WashingMachineSos>(cancellationToken);
        return soses.ToArray();
    }
}