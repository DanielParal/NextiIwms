using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Manufactures.Commands.CreateManufacture;
using Nexticz.Module.Mmo.Settings.Application.Manufactures.Queries.GetManufactures;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Seeds.SeedHandlers;

internal class ManufactureSeedHandler(ISender sender, ILogger<ManufactureSeedHandler> logger)
    : BaseSeedHandler<GetManufacturesQuery, Manufacture>(sender, logger), ISeedRunner
{
    private readonly ISender _sender = sender;

    private static Dictionary<string, string> Manufactures => new()
    {
        { "PLV", "PLV" },
        { "FRL", "FRL" },
        { "CP3", "CP3" },
        { "CP4", "CP4" },
        { "CPN5", "CPN5" },
        { "CB4", "CB4" },
        { "RAIL", "RAIL" },
        { "CPN6", "CPN6" },
        { "CP4I_ME", "CP4i ME" },
        { "EM", "EM" },
        { "CB6", "CB6" },
        { "DRV", "DRV" },
        { "PCV3", "PCV3" },
        { "SIS", "SIS" },
        { "PLV1", "PLV1" },
        { "PLV5", "PLV5" },
        { "CRI", "CRI" }
    };

    public async Task RunSeedAsync(CancellationToken cancellationToken)
    {
        if (!await EnsureNoExistingDataAsync(new GetManufacturesQuery(new BaseFilteringParams()), cancellationToken))
            return;

        foreach (var manufacture in Manufactures)
        {
            var createCommand = new CreateManufactureCommand(manufacture.Key, manufacture.Value);
            await _sender.Send(createCommand, cancellationToken);
        }
    }
}