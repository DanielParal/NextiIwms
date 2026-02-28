using System.Reflection;
using Marten;

namespace Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

public abstract class MartenConfigurationOrchestrator
{
    public static void RegisterMartenConfigurators(StoreOptions options, Assembly targetAssembly)
    {
        var configurators = targetAssembly
            .GetTypes()
            .Where(t => typeof(IMartenConfigurator).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IMartenConfigurator>();

        foreach (var configurator in configurators)
        {
            configurator.Configure(options);
        }
    }
    
    public static void RegisterMartenEvents(StoreOptions options, Assembly targetAssembly)
    {
        var eventTypes = targetAssembly
            .GetExportedTypes()
            .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
            .Where(t => typeof(IMartenEvent).IsAssignableFrom(t))
            .Distinct()
            .ToArray();

        if (eventTypes.Length > 0)
            options.Events.AddEventTypes(eventTypes);
    }

    public static string PluralizeTableName<T>(string suffix = "s")
    {
        return typeof(T).Name.ToLower() + suffix;
    }
}