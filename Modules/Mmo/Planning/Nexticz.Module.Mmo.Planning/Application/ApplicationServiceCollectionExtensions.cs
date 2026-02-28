using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Planning.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Planning.Application.PipelineBehaviors;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateBatches;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Orchestrators.ActivateBatch;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Orchestrators.MoveBatchToAnotherQueue;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Orchestrators.SplitBatch;
using Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables;

namespace Nexticz.Module.Mmo.Planning.Application;

internal static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions));
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
            options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PlanningPostCommandBehavior<,>));
        });
        
        services.AddValidatorsFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions), includeInternalTypes: true);

        services.AddScoped<ICreateBatchesBaseCommandHandler, CreateBatchesBaseCommandHandler>();
        services.AddScoped<WashingMachineTimeTableFactory>();
        services.AddScoped<IPlanningNotificationCollector, PlanningNotificationCollector>();
        
        services.AddScoped<ISplitBatchOrchestrator, SplitBatchOrchestrator>();
        services.AddScoped<IMoveBatchToAnotherQueueOrchestrator, MoveBatchToAnotherQueueOrchestrator>();
        services.AddScoped<IActivateBatchOrchestrator, ActivateBatchOrchestrator>();
        
        return services;
    }
}