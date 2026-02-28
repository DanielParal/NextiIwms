using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.KitTypes.Commands.CreateKitType;
using Nexticz.Module.Mmo.Settings.Application.KitTypes.Queries.GetKitTypes;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Seeds.SeedHandlers;

internal class KitTypeSeedHandler(ISender sender, ILogger<KitTypeSeedHandler> logger)
    : BaseSeedHandler<GetKitTypesQuery, KitType>(sender, logger), ISeedRunner
{
    private readonly ISender _sender = sender;

    private static Dictionary<string, string> KitTypes => new()
    {
        { "J", "Jipocar" },
        { "Z", "Zákaznický" }
    };
    
    public async Task RunSeedAsync(CancellationToken cancellationToken)
    {
        if (!await EnsureNoExistingDataAsync(new GetKitTypesQuery(new BaseFilteringParams()), cancellationToken))
            return;
        
        foreach (var kitType in KitTypes)
        {
            var createKitTypeCommand = new CreateKitTypeCommand(kitType.Key, kitType.Value);
            await _sender.Send(createKitTypeCommand, cancellationToken);
        }
    }
}