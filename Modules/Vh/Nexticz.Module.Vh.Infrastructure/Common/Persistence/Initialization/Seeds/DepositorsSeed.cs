namespace Nexticz.Module.Vh.Infrastructure.Common.Persistence.Initialization.Seeds;

public class DepositorsSeed
{
    private readonly IServiceProvider _serviceProvider;

    public DepositorsSeed(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task RunSeed()
    {
    }
}