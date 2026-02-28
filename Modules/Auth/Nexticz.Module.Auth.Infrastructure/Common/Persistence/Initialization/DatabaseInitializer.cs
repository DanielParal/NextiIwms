using Microsoft.Extensions.Configuration;
using Nexticz.Module.Auth.Infrastructure.Common.Persistence.Initialization.Seeds;

namespace Nexticz.Module.Auth.Infrastructure.Common.Persistence.Initialization;

public class DatabaseInitializer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public DatabaseInitializer(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public async Task RunSeed()
    {
        await new AppRoleSeed(_serviceProvider, _configuration).RunSeed();
        await new AppUserSeed(_serviceProvider).RunSeed(); 
        // await new TranslationsSeed(_serviceProvider).RunSeed();
    }
}