using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingsByPackagingCirculationCode;

internal class GetPackagingsByPackagingCirculationCodeQueryQueryHandler (IPackagingReadOnlyRepository packagingReadOnlyRepository) 
    : IRequestHandler<GetPackagingsByPackagingCirculationCodeQuery, IReadOnlyList<Packaging>>
{
    public async Task<IReadOnlyList<Packaging>> Handle(GetPackagingsByPackagingCirculationCodeQuery request, CancellationToken cancellationToken)
    {
        return await packagingReadOnlyRepository.GetPackagingsByPackagingCirculationCodeAsync(request.PackagingCirculationCode, cancellationToken);
    }
}