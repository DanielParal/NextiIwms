using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Portal.Application.Modules.Commands.CreateModule;
using Nexticz.Module.Portal.Contracts.Modules;

namespace Nexticz.Module.Portal.Application.Seeds.Orchestrators;

internal class SeedOrchestrator(
    ISender sender,
    ILogger<SeedOrchestrator> logger) : ISeedOrchestrator
{
    private static readonly List<CreateModuleRequest> ModuleRequests = 
    [
        new ("vh", "fa-duotone fa-person-digging", "/vh/master-preview-window", true),
        new ("rzno", "fa-duotone fa-truck-fast", "/rzno/home", false),
        new ("por", "fa-duotone fa-building-wheat", "/por/moves", false),
        new ("auth", "fa-duotone fa-users-gear", "/auth/accounts", true),
        new ("lang", "fa-duotone fa-earth-europe", "/lang/settings", true),
        new ("portal", "fa-duotone fa-puzzle", "/portal/settings/modules", true),
        new ("mmo", "fa-duotone fa-solid fa-washing-machine", "/mmo/planning", true),
        new ("sign", "fa-duotone fa-solid fa-pen-field", "/sign", true),
    ];
    
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        foreach (var moduleRequest in ModuleRequests)
        {
            await sender.Send(new CreateModuleCommand(moduleRequest), cancellationToken);
        }
        
        logger.LogInformation("PORTAL - Seeding completed");
    }
}