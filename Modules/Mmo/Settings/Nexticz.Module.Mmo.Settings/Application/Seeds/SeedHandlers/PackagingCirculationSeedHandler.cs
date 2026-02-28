using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Commands.CreatePackagingCirculation;
using Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Queries.GetPackagingCirculations;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Seeds.SeedHandlers;

internal class PackagingCirculationSeedHandler(ISender sender, ILogger<PackagingCirculationSeedHandler> logger)
    : BaseSeedHandler<GetPackagingCirculationsQuery, PackagingCirculation>(sender, logger), ISeedRunner
{
    private readonly ISender _sender = sender;

    private static Dictionary<string, string> PackagingCirculations => new()
    {
        { "MWEG", "MWEG" },
        { "VERP", "VERP" }
    };

    public async Task RunSeedAsync(CancellationToken cancellationToken)
    {
        if (!await EnsureNoExistingDataAsync(new GetPackagingCirculationsQuery(new BaseFilteringParams()),
                cancellationToken))
            return;

        foreach (var packagingCirculation in PackagingCirculations)
        {
            var createCommand =
                new CreatePackagingCirculationCommand(packagingCirculation.Key, packagingCirculation.Value);
            await _sender.Send(createCommand, cancellationToken);
        }
    }
}