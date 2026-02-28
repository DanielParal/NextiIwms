using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Presentation.Depositors;
using Nexticz.Module.Mmo.Settings.Presentation.Exports;
using Nexticz.Module.Mmo.Settings.Presentation.HistoryEvents;
using Nexticz.Module.Mmo.Settings.Presentation.Imports;
using Nexticz.Module.Mmo.Settings.Presentation.Kits;
using Nexticz.Module.Mmo.Settings.Presentation.KitSapDefinitions;
using Nexticz.Module.Mmo.Settings.Presentation.KitTypes;
using Nexticz.Module.Mmo.Settings.Presentation.Manufactures;
using Nexticz.Module.Mmo.Settings.Presentation.OpenApiContracts;
using Nexticz.Module.Mmo.Settings.Presentation.PackagingCirculations;
using Nexticz.Module.Mmo.Settings.Presentation.Packagings;
using Nexticz.Module.Mmo.Settings.Presentation.PackagingTypes;
using Nexticz.Module.Mmo.Settings.Presentation.Projections;
using Nexticz.Module.Mmo.Settings.Presentation.Seeds;
using Nexticz.Module.Mmo.Settings.Presentation.SpecialInformations;
using Nexticz.Module.Mmo.Settings.Presentation.WashingMachines;
using Nexticz.Module.Mmo.SharedKernel.Security;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Presentation.Constants;
using Nexticz.Module.Mmo.Settings.Presentation.InactivityTypes;
using Nexticz.Module.Mmo.Settings.Presentation.Users;
using Nexticz.Module.Mmo.Settings.Presentation.Workers;

namespace Nexticz.Module.Mmo.Settings.Presentation;

internal static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapMmoMapOpenApiContractEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.OpenApiContractEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(AuthorizationHelper.Role.Developer)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapOpenApiContractEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoKitEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.KitEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings), nameof(Permission.MmoMaster)])
                )
            )
            .MapKitsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionMmoKitEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.KitEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapAnyPermissionKitsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoDepositorEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.DepositorEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings)])
                )
            )
            .MapDepositorsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionMmoDepositorEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.DepositorEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapAnyPermissionDepositorsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoPackagingTypesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.PackagingTypeEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings)])
                )
            )
            .MapPackagingTypesEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionMmoPackagingTypesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.PackagingTypeEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapAnyPermissionPackagingTypesEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoPackagingsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.PackagingEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings), nameof(Permission.MmoMaster)])
                )
            )
            .MapPackagingsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionMmoPackagingsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.PackagingEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapAnyPermissionPackagingsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoManufacturesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.ManufactureEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings)])
                )
            )
            .MapManufacturesEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionMmoManufacturesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.ManufactureEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapAnyPermissionManufacturesEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoPackagingCirculationsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.PackagingCirculationEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings)])
                )
            )
            .MapPackagingCirculationsEndpoints();
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionMmoPackagingCirculationsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.PackagingCirculationEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapAnyPermissionPackagingCirculationsEndpoints();
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoKitTypesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.KitTypeEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings)])
                )
            )
            .MapKitTypesEndpoints();
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionMmoKitTypesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.KitTypeEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapAnyPermissionKitTypesEndpoints();
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoMapKitSapDefinitionsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.KitSapDefinitionEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings)])
                )
            )
            .MapKitSapDefinitionsEndpoints();
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionMmoMapKitSapDefinitionsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.KitSapDefinitionEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapAnyPermissionKitSapDefinitionsEndpoints();
        
        return builder;
    }
    
    
    
    public static IEndpointRouteBuilder MapMmoWashingMachinesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.WashingMachineEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings)])
                )
            )
            .MapWashingMachinesEndpoints();
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionMmoWashingMachinesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.WashingMachineEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapAnyPermissionWashingMachinesEndpoints();
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoImportsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.ImportEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings)])
                )
            )
            .MapImportsEndpoints();
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoExportsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.ExportEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings)])
                )
            )
            .MapExportsEndpoints();
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoHistoryEventsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.HistoryEventEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings), nameof(Permission.MmoMaster)])
                )
            )
            .MapHistoryEventsEndpoints();
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoSeedsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.SeedEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(AuthorizationHelper.Role.Developer)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapSeedsEndpoints();
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoMapProjectionEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.ProjectionEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(AuthorizationHelper.Role.Developer)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapProjectionEndpoints();
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapConstantsEndpoints(this IEndpointRouteBuilder builder)
    {
        ConstantEndpointsExtensions.MapConstantsEndpoints(builder.NewVersionedApi(nameof(SettingsEndpoints.ConstantEndpoints))
                .RequireAuthorization(policy =>
                    policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                        context,
                        [nameof(Role.MmoMember)],
                        [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings)])
                    )
                ));
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapDeveloperOnlyConstantsEndpoints(this IEndpointRouteBuilder builder)
    {
        ConstantEndpointsExtensions.MapDeveloperOnlyConstantsEndpoints(builder.NewVersionedApi(nameof(SettingsEndpoints.ConstantEndpoints))
                .RequireAuthorization(policy =>
                    policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                        context,
                        [nameof(AuthorizationHelper.Role.Developer)],
                        [nameof(AuthorizationHelper.Permission.Any)])
                    )
                ));
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapWorkersEndpoints(this IEndpointRouteBuilder builder)
    {
        WorkerEndpointsExtensions.MapWorkersEndpoints(builder.NewVersionedApi(nameof(SettingsEndpoints.WorkerEndpoints))
                .RequireAuthorization(policy =>
                    policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                        context,
                        [nameof(Role.MmoMember)],
                        [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings), nameof(Permission.MmoMaster)])
                    )
                ));
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapCompletionPermissionWorkersEndpoints(this IEndpointRouteBuilder builder)
    {
        WorkerEndpointsExtensions.MapCompletionPermissionWorkersEndpoints(builder.NewVersionedApi(nameof(SettingsEndpoints.WorkerEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings), nameof(Permission.MmoMaster), nameof(Permission.MmoCompletion)])
                )
            ));
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapInactivityTypesEndpoints(this IEndpointRouteBuilder builder)
    {
        InactivityTypeEndpointsExtensions.MapInactivityTypesEndpoints(builder.NewVersionedApi(nameof(SettingsEndpoints.InactivityTypeEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings)])
                )
            ));
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionMmoInactivityTypesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.InactivityTypeEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapAnyPermissionInactivityTypesEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder builder)
    {
        UserEndpointsExtensions.MapUsersEndpoints(builder.NewVersionedApi(nameof(SettingsEndpoints.UserEndpoints))
                .RequireAuthorization(policy =>
                    policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                        context,
                        [nameof(Role.MmoMember)],
                        [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings)])
                    )
                ));
        
        return builder;
    }
    
    public static IEndpointRouteBuilder MapMmoSpecialInformationsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.SpecialInformationEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.MmoMember)],
                    [nameof(Permission.MmoManageAll), nameof(Permission.MmoManageSettings), nameof(Permission.MmoMaster)])
                )
            )
            .MapSpecialInformationsEndpoints();

        return builder;
    }
    
    
}