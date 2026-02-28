using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings.Queries;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingByCode;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingResponseByCode;

internal class GetPackagingResponseByCodeQueryHandler(ISender sender) : IRequestHandler<GetPackagingResponseByCodeQuery, ErrorOr<PackagingResponse>>
{
    public async Task<ErrorOr<PackagingResponse>> Handle(GetPackagingResponseByCodeQuery request, CancellationToken cancellationToken)
    {
        var packaging = await sender.Send(new GetPackagingByCodeQuery(request.Code), cancellationToken);
        
        if (packaging.IsError)
            return packaging.Errors;
        
        return PackagingResponseFactory.Create(packaging.Value);
    }
}