using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Interfaces;

internal interface ISpecialInformationReadOnlyRepository
{
    Task<SpecialInformation?> GetByTitleAsync(string title, CancellationToken cancellationToken);
    Task<IReadOnlyList<SpecialInformation>> GetSpecialInformationsByIdsAsync(Guid[] ids, CancellationToken cancellationToken);
}