using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Interfaces;

internal interface IKitSapDefinitionReadOnlyRepository
{
    Task<KitSapDefinition?> GetByCodeAsync(string code, CancellationToken cancellationToken);
}