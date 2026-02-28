using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Queries.GetInactivityTypeByName;

internal class GetInactivityTypeByNameQueryHandler(
    IInactivityTypeReadOnlyRepository inactivityTypeReadOnlyRepository) 
    : IRequestHandler<GetInactivityTypeByNameQuery, ErrorOr<InactivityType>>
{
    public async Task<ErrorOr<InactivityType>> Handle(GetInactivityTypeByNameQuery request, CancellationToken cancellationToken)
    {
        var inactivityType = await inactivityTypeReadOnlyRepository.GetByNameAsync(request.Name, cancellationToken);
        
        if (inactivityType is null)
            return InactivityTypeErrors.NotFoundInactivityTypeWithName;
        
        return inactivityType;
    }
}