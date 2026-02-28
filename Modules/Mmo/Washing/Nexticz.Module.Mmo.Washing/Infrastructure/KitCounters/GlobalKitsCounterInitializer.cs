using Microsoft.Extensions.Hosting;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.KitCounters;

internal class GlobalKitsCounterInitializer(IGlobalKitsCounter counter, IWashingDocumentStore store) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await counter.InitializeAsync(store.IdentitySession(), cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}