using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Queries.GetInactivityTypeById;

internal class GetInactivityTypeByIdQueryHandler(
    ISettingsReadOnlyEventStoreRepository settingsReadOnlyEventStoreRepository) 
    : IRequestHandler<GetInactivityTypeByIdQuery, ErrorOr<InactivityType>>
{
    public async Task<ErrorOr<InactivityType>> Handle(GetInactivityTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var inactivityType = await settingsReadOnlyEventStoreRepository.GetByIdAsync<InactivityType>(request.Id, cancellationToken);
        if (inactivityType is null)
            return InactivityTypeErrors.NotFoundInactivityTypeWithId;
        
        return inactivityType;
    }
}