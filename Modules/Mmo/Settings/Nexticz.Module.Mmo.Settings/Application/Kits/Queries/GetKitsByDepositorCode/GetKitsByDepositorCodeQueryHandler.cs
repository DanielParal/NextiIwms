using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsByDepositorCode;

internal class GetKitsByDepositorCodeQueryHandler (IKitReadOnlyRepository kitReadOnlyRepository)
    : IRequestHandler<GetKitsByDepositorCodeQuery, IReadOnlyList<Kit>>
{
    public async Task<IReadOnlyList<Kit>> Handle(GetKitsByDepositorCodeQuery request, CancellationToken cancellationToken)
    {
        return await kitReadOnlyRepository.GetKitsByDepositorCodeAsync(request.DepositorCode, cancellationToken);
    }
}