using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingsByCodes;

internal class GetPackagingsByCodesQueryHandler (IPackagingReadOnlyRepository packagingReadOnlyRepository) 
    : IRequestHandler<GetPackagingsByCodesQuery, IReadOnlyList<Packaging>>
{
    public async Task<IReadOnlyList<Packaging>> Handle(GetPackagingsByCodesQuery request, CancellationToken cancellationToken)
    {
        return await packagingReadOnlyRepository.GetPackagingsByCodesAsync(request.Codes, cancellationToken);
    }
}