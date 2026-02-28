using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Interfaces;

internal interface IKitReadOnlyRepository
{
    Task<Kit?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<IReadOnlyList<Kit>> GetKitsByKitTypeCodeAsync(string kitTypeCode, CancellationToken cancellationToken);
    Task<IReadOnlyList<Kit>> GetKitsByKitSapDefinitionCodeAsync(string kitSapDefinitionCode, CancellationToken cancellationToken);
    Task<IReadOnlyList<Kit>> GetKitsByManufactureCodeAsync(string manufactureCode, CancellationToken cancellationToken);
    Task<IReadOnlyList<Kit>> GetKitsByDepositorCodeAsync(string depositorCode, CancellationToken cancellationToken);
    Task<IReadOnlyList<Kit>> GetKitsByPackagingCode(string packagingCode, CancellationToken cancellationToken);
    Task<IReadOnlyList<Kit>> GetKitsBySpecialInformationId(Guid id, CancellationToken cancellationToken);
}