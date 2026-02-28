using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Interfaces;

internal interface IInactivityTypeReadOnlyRepository
{
    Task<InactivityType?> GetByNameAsync(string name, CancellationToken cancellationToken);
}