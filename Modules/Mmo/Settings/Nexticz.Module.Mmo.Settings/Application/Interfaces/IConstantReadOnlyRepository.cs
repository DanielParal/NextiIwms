using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Interfaces;

internal interface IConstantReadOnlyRepository
{
    Task<Constant?> GetByKeyAsync(string key, CancellationToken cancellationToken);
}