namespace Nexticz.Module.Vh.Infrastructure.Common.Persistence.Initialization.Seeds;

public class DepositorsGroupsSeed
{
    private readonly IServiceProvider _serviceProvider;

    public DepositorsGroupsSeed(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task RunSeed()
    {
        
    }
}