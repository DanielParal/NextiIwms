using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Queries.GetPackagingTypeByCode;

internal class GetPackagingTypeByCodeQueryHandler (IPackagingTypeReadOnlyRepository packagingTypeReadOnlyRepository)
    : IRequestHandler<GetPackagingTypeByCodeQuery, ErrorOr<PackagingType>>
{
    public async Task<ErrorOr<PackagingType>> Handle(GetPackagingTypeByCodeQuery request, CancellationToken cancellationToken)
    {
        var packageType = await packagingTypeReadOnlyRepository.GetByCodeAsync(request.Code, cancellationToken);

        if (packageType == null)
            return PackagingTypeErrors.CodeDoesNotExist;

        return packageType;
    }
}