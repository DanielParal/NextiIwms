using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Interfaces;

internal interface IPackagingReadOnlyRepository
{
    Task<Packaging?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<IReadOnlyList<Packaging>> GetPackagingsByCodesAsync(string[] codes, CancellationToken cancellationToken);
    Task<IReadOnlyList<Packaging>> GetPackagingsByDepositorCodeAsync(string depositorCode, CancellationToken cancellationToken);
    Task<IReadOnlyList<Packaging>> GetPackagingsByPackagingTypeCodeAsync(string packagingTypeCode, CancellationToken cancellationToken);
    Task<IReadOnlyList<Packaging>> GetPackagingsByPackagingCirculationCodeAsync(string packagingCirculationCode, CancellationToken cancellationToken);
}