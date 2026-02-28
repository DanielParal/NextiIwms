namespace Nexticz.Module.Vh.Infrastructure.Common.Persistence.Initialization.Seeds;

public class BandRewardsSeed
{
    private readonly IServiceProvider _serviceProvider;

    public BandRewardsSeed(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task RunSeed()
    {
    }
}