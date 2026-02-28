namespace Nexticz.Module.Vh.Infrastructure.Common.Persistence.Initialization.Seeds;

public class NonDispensingActivitiesSeed
{
    private readonly IServiceProvider _serviceProvider;

    public NonDispensingActivitiesSeed(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task RunSeed()
    {
    }
}