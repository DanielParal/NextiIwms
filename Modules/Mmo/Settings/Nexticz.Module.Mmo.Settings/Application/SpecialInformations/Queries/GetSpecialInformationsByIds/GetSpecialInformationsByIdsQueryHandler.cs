using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationsByIds;

internal class GetSpecialInformationsByIdsQueryHandler(
    ISpecialInformationReadOnlyRepository specialInformationReadOnlyRepository) 
    : IRequestHandler<GetSpecialInformationsByIdsQuery, SpecialInformation[]>
{
    public async Task<SpecialInformation[]> Handle(GetSpecialInformationsByIdsQuery request, CancellationToken cancellationToken)
    {
        var specialInformations =
            await specialInformationReadOnlyRepository.GetSpecialInformationsByIdsAsync(request.Ids, cancellationToken);
        return specialInformations.ToArray();
    }
}