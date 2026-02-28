using Marten;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.SpecialInformations;

internal class SpecialInformationReadOnlyRepository(ISettingsReadOnlyEventStoreRepository settingsReadOnlyEventStoreRepository) 
    : ISpecialInformationReadOnlyRepository
{
    public async Task<SpecialInformation?> GetByTitleAsync(string title, CancellationToken cancellationToken)
    {
        var trimmedTitle = title.Trim();
        return await settingsReadOnlyEventStoreRepository.GetFirstByConditionAsync<SpecialInformation>(
            x => x.Title.Equals(trimmedTitle, StringComparison.InvariantCultureIgnoreCase), cancellationToken);
    }

    public async Task<IReadOnlyList<SpecialInformation>> GetSpecialInformationsByIdsAsync(Guid[] ids, CancellationToken cancellationToken)
    {
        return await settingsReadOnlyEventStoreRepository.GetAllByConditionAsync<SpecialInformation>(
            x => x.Id.IsOneOf(ids), cancellationToken);
    }
}