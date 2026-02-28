using Nexticz.Module.Vh.Infrastructure.Common.Persistence.Initialization.Seeds;

namespace Nexticz.Module.Vh.Infrastructure.Common.Persistence.Initialization;

public class DatabaseInitializer
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task RunSeed()
    {
        await new CentersSeed(_serviceProvider).RunSeed();
        await new DepositorsSeed(_serviceProvider).RunSeed();
        await new DepositorsGroupsSeed(_serviceProvider).RunSeed();
        await new WorkersSeed(_serviceProvider).RunSeed();
        await new ActivityCategoriesSeed(_serviceProvider).RunSeed();
        await new NonDispensingActivitiesSeed(_serviceProvider).RunSeed();
        await new SystemActivitiesSeed(_serviceProvider).RunSeed();
        await new AssortmentsSeed(_serviceProvider).RunSeed();
        await new BandRewardsSeed(_serviceProvider).RunSeed();
        await new PartnersSeed(_serviceProvider).RunSeed();
    }
}