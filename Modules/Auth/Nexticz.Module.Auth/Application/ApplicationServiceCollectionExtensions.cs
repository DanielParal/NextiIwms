using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Auth.Application.Users.Orchestrators;

namespace Nexticz.Module.Auth.Application;

internal static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // services.AddMediatR(options =>
        // {
        //     options.RegisterServicesFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions));
        //     options.AddOpenBehavior(typeof(ValidationBehavior<,>));
        // });
        
        services.AddScoped<ISyncUserOrchestrator, SyncUserOrchestrator>();
        
        return services;
    }
}