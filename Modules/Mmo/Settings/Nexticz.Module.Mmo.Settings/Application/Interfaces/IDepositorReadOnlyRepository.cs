using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Interfaces;

internal interface IDepositorReadOnlyRepository
{
    Task<Depositor?> GetByCodeAsync(string code, CancellationToken cancellationToken);
}