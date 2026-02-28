using Nexticz.Module.Lang.Infrastructure.Common.Persistence.Initialization.Seeds;

namespace Nexticz.Module.Lang.Infrastructure.Common.Persistence.Initialization;

public class DatabaseInitializer
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task RunSeed()
    {
        await new LanguagesSeed(_serviceProvider).RunSeed();
        await new TranslationsSeed(_serviceProvider).RunSeed();
    }
}