using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsByPackagingCode;

internal class GetKitsByPackagingCodeQueryHandler (IKitReadOnlyRepository kitReadOnlyRepository)
    : IRequestHandler<GetKitsByPackagingCodeQuery, IReadOnlyList<Kit>>
{
    public async Task<IReadOnlyList<Kit>> Handle(GetKitsByPackagingCodeQuery request, CancellationToken cancellationToken)
    {
        return await kitReadOnlyRepository.GetKitsByPackagingCode(request.PackagingCode, cancellationToken);
    }
}