namespace Nexticz.Module.Vh.Infrastructure.Common.Persistence.Initialization.Seeds;

public class ActivityCategoriesSeed
{
    private readonly IServiceProvider _serviceProvider;

    public ActivityCategoriesSeed(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task RunSeed()
    {
    }
}