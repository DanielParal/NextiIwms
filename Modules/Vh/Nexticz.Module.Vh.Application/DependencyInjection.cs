using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Vh.Application.Common.Behavior;
using Nexticz.Module.Vh.Application.LoadedActivities.PdaReaderRawEvents;
using Nexticz.Module.Vh.Application.LoadedActivities.PdaReaderRawEvents.MetadataCalculators;
using Nexticz.Module.Vh.Application.LoadedActivities.PdaReaderRawEvents.MetadataHelpers;
using Nexticz.Module.Vh.Application.VhUsers.Orchestrators;

namespace Nexticz.Module.Vh.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddVhApplication(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection));
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));
        services.AddPdaReaderRawEvents();

        services.AddScoped<IUserChangedOrchestrator, UserChangedOrchestrator>();
        
        return services;
    }

    private static IServiceCollection AddPdaReaderRawEvents(this IServiceCollection services)
    {
        return services
            .AddScoped<IPdaReaderRawEventsHandlerSelector, PdaReaderRawEventsHandlerSelector>()
            .AddScoped<IPdaReaderRawEventsHandler, PdaReaderRawEventsMyStockDbHandler>()
            .AddScoped<IPdaReaderRawEventsHandler, PdaReaderRawEventsMyStockXlsxHandler>()
            .AddScoped<IPdaReaderRawEventsHandler, PdaReaderRawEventsSagDbHandler>()
            .AddScoped<IPdaReaderRawEventsHandler, PdaReaderRawEventsSagCsvHandler>()
            .AddScoped<IPdaReaderRawEventsHandler, PdaReaderRawEventsIwmsDbHandler>()
            .AddScoped<IPdaReaderRawEventsHandler, PdaReaderRawEventsIwmsCsvHandler>()
            .AddTransient<IwmsHandlersCalculators>()
            .AddTransient<MyStockHandlersCalculators>()
            .AddTransient<SagHandlersCalculators>();
    }
}