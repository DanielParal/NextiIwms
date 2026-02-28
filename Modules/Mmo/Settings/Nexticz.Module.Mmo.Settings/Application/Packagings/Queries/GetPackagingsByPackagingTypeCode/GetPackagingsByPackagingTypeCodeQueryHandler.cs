using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingsByPackagingTypeCode;

internal class GetPackagingsByPackagingTypeCodeQueryHandler (IPackagingReadOnlyRepository packagingReadOnlyRepository)
    : IRequestHandler<GetPackagingsByPackagingTypeCodeQuery, IReadOnlyList<Packaging>>
{
    public async Task<IReadOnlyList<Packaging>> Handle(GetPackagingsByPackagingTypeCodeQuery request, CancellationToken cancellationToken)
    {
        return await packagingReadOnlyRepository.GetPackagingsByPackagingTypeCodeAsync(request.PackagingTypeCode, cancellationToken);
    }
}