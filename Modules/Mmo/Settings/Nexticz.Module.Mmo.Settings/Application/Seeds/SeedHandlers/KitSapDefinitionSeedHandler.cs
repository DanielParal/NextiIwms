using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Commands.CreateKitSapDefinition;
using Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Queries.GetKitSapDefinitions;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Seeds.SeedHandlers;

internal class KitSapDefinitionSeedHandler(ISender sender, ILogger<KitTypeSeedHandler> logger)
    : BaseSeedHandler<GetKitSapDefinitionsQuery, KitSapDefinition>(sender, logger), ISeedRunner
{
    private readonly ISender _sender = sender;

    private static Dictionary<string, string> KitSapDefinitions => new()
    {
        { "KIT_03", "KIT 03" },
        { "KIT_04", "KIT 04" }
    };
    
    public async Task RunSeedAsync(CancellationToken cancellationToken)
    {
        if (!await EnsureNoExistingDataAsync(new GetKitSapDefinitionsQuery(new BaseFilteringParams()), cancellationToken))
            return;
        
        foreach (var kitType in KitSapDefinitions)
        {
            var createKitTypeCommand = new CreateKitSapDefinitionCommand(kitType.Key, kitType.Value);
            await _sender.Send(createKitTypeCommand, cancellationToken);
        }
    }
}