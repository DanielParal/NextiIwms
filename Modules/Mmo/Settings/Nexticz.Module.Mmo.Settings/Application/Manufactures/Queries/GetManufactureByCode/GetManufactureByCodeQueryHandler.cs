using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Manufactures.Queries.GetManufactureByCode;

internal class GetManufactureByCodeQueryHandler (
    IManufactureReadOnlyRepository manufactureReadOnlyRepository
    )
    : IRequestHandler<GetManufactureByCodeQuery, ErrorOr<Manufacture>>
{
    public async Task<ErrorOr<Manufacture>> Handle(GetManufactureByCodeQuery request, CancellationToken cancellationToken)
    {
        var manufacture = await manufactureReadOnlyRepository.GetByCodeAsync(request.Code, cancellationToken);

        if (manufacture is null) 
            return ManufactureErrors.CodeDoesNotExist;
        
        return manufacture;
    }
}