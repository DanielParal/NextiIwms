using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationByTitle;

internal class GetSpecialInformationByTitleQueryHandler(
    ISpecialInformationReadOnlyRepository specialInformationReadOnlyRepository) 
    : IRequestHandler<GetSpecialInformationByTitleQuery, ErrorOr<SpecialInformation>>
{
    public async Task<ErrorOr<SpecialInformation>> Handle(GetSpecialInformationByTitleQuery request, CancellationToken cancellationToken)
    {
        var specialInformation = await specialInformationReadOnlyRepository.GetByTitleAsync(request.Title, cancellationToken);
        
        if (specialInformation is null)
            return SpecialInformationErrors.SpecialInformationNotFound;
        
        return specialInformation;
    }
}