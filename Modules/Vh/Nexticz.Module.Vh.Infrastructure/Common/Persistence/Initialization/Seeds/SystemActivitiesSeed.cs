namespace Nexticz.Module.Vh.Infrastructure.Common.Persistence.Initialization.Seeds;

public class SystemActivitiesSeed
{
    private readonly IServiceProvider _serviceProvider;

    public SystemActivitiesSeed(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task RunSeed()
    {
    }
}