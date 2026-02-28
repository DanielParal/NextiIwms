using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Queries.GetPackagingTypesByCodes;

internal class GetPackagingTypesByCodesQueryHandler (IPackagingTypeReadOnlyRepository packagingTypeReadOnlyRepository)
    : IRequestHandler<GetPackagingTypesByCodesQuery, IReadOnlyList<PackagingType>>
{
    public async Task<IReadOnlyList<PackagingType>> Handle(GetPackagingTypesByCodesQuery request, CancellationToken cancellationToken)
    {
        return await packagingTypeReadOnlyRepository.GetPackagingTypesByCodesAsync(request.Codes, cancellationToken);
    }
}