using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Depositors.Commands.CreateDepositor;
using Nexticz.Module.Mmo.Settings.Application.Depositors.Queries.GetDepositors;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Seeds.SeedHandlers;

internal class DepositorSeedHandler(ISender sender, ILogger<DepositorSeedHandler> logger)
    : BaseSeedHandler<GetDepositorsQuery, Depositor>(sender, logger), ISeedRunner
{
    private readonly ISender _sender = sender;

    public async Task RunSeedAsync(CancellationToken cancellationToken)
    {
        if (!await EnsureNoExistingDataAsync(new GetDepositorsQuery(new BaseFilteringParams()), cancellationToken))
            return;
        
        var createDepositorCommand = new CreateDepositorCommand("BO", "Bosh diesel", "*{{KitNumber}}401KOMPLETY*");
        await _sender.Send(createDepositorCommand, cancellationToken);
    }
}