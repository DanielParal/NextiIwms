using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Module.Mmo.Drying.Application.Interfaces;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Drying.Application.Grouping;
using Nexticz.Module.Mmo.Drying.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Drying.Application.PipelineBehaviors;

namespace Nexticz.Module.Mmo.Drying.Application;

internal static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions));
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
            options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(DryingPostCommandBehavior<,>));
        });
        
        services.AddValidatorsFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions), includeInternalTypes: true);
        
        services.AddScoped<IDryingGroupedResultHandler, DryingGroupedResultHandler>();
        services.AddScoped<IDryingNotificationCollector, DryingNotificationCollector>();
        
        return services;
    }
}