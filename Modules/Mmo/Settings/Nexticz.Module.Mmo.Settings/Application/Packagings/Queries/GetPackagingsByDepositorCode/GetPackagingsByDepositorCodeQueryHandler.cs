using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingsByDepositorCode;

internal class GetPackagingsByDepositorCodeQueryHandler (IPackagingReadOnlyRepository packagingReadOnlyRepository) 
    : IRequestHandler<GetPackagingsByDepositorCodeQuery, IReadOnlyList<Packaging>>
{
    public async Task<IReadOnlyList<Packaging>> Handle(GetPackagingsByDepositorCodeQuery request, CancellationToken cancellationToken)
    {
        return await packagingReadOnlyRepository.GetPackagingsByDepositorCodeAsync(request.DepositorCode, cancellationToken);
    }
}