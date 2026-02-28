using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingByCode;

internal class GetPackagingByCodeQueryHandler (IPackagingReadOnlyRepository packagingReadOnlyRepository)
    : IRequestHandler<GetPackagingByCodeQuery, ErrorOr<Domain.PackagingAggregate.Packaging>>
{
    public async Task<ErrorOr<Domain.PackagingAggregate.Packaging>> Handle(GetPackagingByCodeQuery request, CancellationToken cancellationToken)
    {
        var packaging = await packagingReadOnlyRepository.GetByCodeAsync(request.Code, cancellationToken);

        if (packaging is null)
            return PackagingErrors.CodeDoesNotExist;
        
        return packaging;
    }
}