using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.DriedKits.Queries.GetDriedKitByIdFromDrying;

internal class GetDriedKitByIdFromDryingQueryHandler(IDriedKitReadOnlyRepository driedKitReadOnlyRepository) 
    : IRequestHandler<GetDriedKitByIdFromDryingQuery, ErrorOr<DriedKit>>
{
    public async Task<ErrorOr<DriedKit>> Handle(GetDriedKitByIdFromDryingQuery request, CancellationToken cancellationToken)
    {
        var driedKit = await driedKitReadOnlyRepository.GetDriedKitByIdFromDryingAsync(request.IdFromDying, cancellationToken);
        if (driedKit == null)
            return DriedKitErrors.DriedKitNotFound;

        return driedKit;
    }
}