using Nexticz.Module.Portal.Contracts.Modules;
using Nexticz.Module.Portal.Domain.ModuleAggregate;

namespace Nexticz.Module.Portal.Presentation.Modules;

internal static class ModuleResponseFactory
{
    public static ModuleResponse Create(Domain.ModuleAggregate.Module module)
    {
        return new ModuleResponse()
        {
            Id = module.Id,
            Name = module.Name,
            Icon = module.Icon,
            BaseUrl = module.BaseUrl,
            IsActive = module.IsActive,
            SortOrder = module.SortOrder
        };
    }
}