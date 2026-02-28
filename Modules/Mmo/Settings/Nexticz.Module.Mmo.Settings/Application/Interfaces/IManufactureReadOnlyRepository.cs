using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Interfaces;

internal interface IManufactureReadOnlyRepository
{
    Task<Manufacture?> GetByCodeAsync(string code, CancellationToken cancellationToken);
}