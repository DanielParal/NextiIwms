using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Commands.CreatePackagingType;
using Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Queries.GetPackagingTypes;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Seeds.SeedHandlers;

internal class PackagingTypeSeedHandler(ISender sender, ILogger<ManufactureSeedHandler> logger)
    : BaseSeedHandler<GetPackagingTypesQuery, PackagingType>(sender, logger), ISeedRunner
{
    private readonly ISender _sender = sender;

    private static Dictionary<string, string> PackagingTypes => new()
    {
        { "ADAPTER_PALETA", "Adapter paleta" },
        { "BLISTR_JEDNORAZOVY", "Blistr jednorázový" },
        { "BLISTR_SAMONOSNY", "Blistr samonosný" },
        { "BLISTR_VNITRNI", "Blistr vnitřní" },
        { "BUBLINKOVA_FOLIE", "Bublinková fólie" },
        { "FIXACE", "Fixace" },
        { "KLT", "KLT" },
        { "KLT_KOVOVE", "KLT kovové" },
        { "KONTEJNER", "Kontejner" },
        { "PALETA_DREVENA", "Paleta dřevěná" },
        { "PALETA_KOVOVA", "Paleta kovová" },
        { "PALETA_PLASTOVA", "Paleta plastová" },
        { "PALETOVA_FIXACE", "Paletová fixace" },
        { "PALETOVY_RAM", "Paletový rám" },
        { "PENOVY_PRIREZ", "Pěnový přířez" },
        { "PROLOZKA", "Proložka" },
        { "PULPALETA_PLASTOVA", "Půlpaleta plastová" },
        { "SACEK_POLYETYLENOVY", "Sáček polyetylénový" },
        { "SKEJT", "Skejt" },
        { "TRANSPORTNI_VOZIK", "Transportní vozík" },
        { "VELKY_PYTEL", "Velký pytel" },
        { "VICKO_KLT", "Víčko KLT" },
        { "VIKO_PALETY", "Víko palety" },
        { "VIKO_PULPALETY", "Víko půlpalety" },
        { "VIKO_SAMONOSNEHO_BLISTRU", "Víko samonosného Blistru" },
        { "ZAJISTOVACI_KLIP", "Zajišťovací klip" }
    };

    public async Task RunSeedAsync(CancellationToken cancellationToken)
    {
        if (!await EnsureNoExistingDataAsync(new GetPackagingTypesQuery(new BaseFilteringParams()), cancellationToken))
            return;

        foreach (var packagingType in PackagingTypes)
        {
            var createCommand = new CreatePackagingTypeCommand(packagingType.Key, packagingType.Value);
            await _sender.Send(createCommand, cancellationToken);
        }
    }
}