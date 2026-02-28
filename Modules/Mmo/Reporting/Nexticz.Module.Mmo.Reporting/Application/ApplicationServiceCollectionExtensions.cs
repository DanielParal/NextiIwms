using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Reporting.Application.Grouping;
using Nexticz.Module.Mmo.Reporting.Application.Kits.Orchestrators;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Orchestrators.ChangeComment;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Orchestrators.ChangeItem;
using Nexticz.Module.Mmo.Reporting.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Reporting.Application.PipelineBehaviors;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Orchestrators;
using Nexticz.Module.Mmo.Reporting.Application.ShiftSettings;

namespace Nexticz.Module.Mmo.Reporting.Application;

internal static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions));
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
            options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ReportingPostCommandBehavior<,>));
        });
        
        services.AddValidatorsFromAssemblyContaining(typeof(ApplicationServiceCollectionExtensions), includeInternalTypes: true);
        
        services.AddScoped<IReportingNotificationCollector, ReportingNotificationCollector>();
        services.AddScoped<IReportingGroupedResultHandler, ReportingGroupedResultHandler>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<IShiftCreationOrchestrator, ShiftCreationOrchestrator>();
        services.AddScoped<ShiftSettingManagerFactory>();
        services.AddScoped<IChangeItemOrchestrator, ChangeItemOrchestrator>();
        services.AddScoped<IChangeCommentOrchestrator, ChangeCommentOrchestrator>();
        
        services.AddScoped<ICreateItemsAfterKitFinishedOrchestrator, CreateItemsAfterKitFinishedOrchestrator>();
        services.AddScoped<IUnapproveShiftOrchestrator, UnapproveShiftOrchestrator>();
        
        return services;
    }
}