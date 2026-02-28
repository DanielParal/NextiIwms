using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.PackagingCirculations;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;

namespace Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Queries.GetPackagingCirculationByCode;

internal class GetPackagingCirculationByCodeQueryHandler(IPackagingCirculationReadOnlyRepository repository) 
    : IRequestHandler<GetPackagingCirculationByCodeQuery, ErrorOr<PackagingCirculation>>
{
    public async Task<ErrorOr<PackagingCirculation>> Handle(GetPackagingCirculationByCodeQuery request, CancellationToken cancellationToken)
    {
        var packageCirculation = await repository.GetByCodeAsync(request.Code, cancellationToken);

        if (packageCirculation is null)
            return PackagingCirculationErrors.CodeDoesNotExist;

        return packageCirculation;
    }
}