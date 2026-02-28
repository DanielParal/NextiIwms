using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationById;

internal class GetSpecialInformationByIdQueryHandler(ISettingsReadOnlyEventStoreRepository settingsReadOnlyEventStoreRepository) 
    : IRequestHandler<GetSpecialInformationByIdQuery, ErrorOr<SpecialInformation>>
{
    public async Task<ErrorOr<SpecialInformation>> Handle(GetSpecialInformationByIdQuery request, CancellationToken cancellationToken)
    {
        var specialInformation = await settingsReadOnlyEventStoreRepository.GetByIdAsync<SpecialInformation>(request.Id, cancellationToken);
        
        if (specialInformation is null)
            return SpecialInformationErrors.SpecialInformationNotFound;

        return specialInformation;
    }
}