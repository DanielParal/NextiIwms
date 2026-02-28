using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Presentation.Constants;
using Nexticz.Module.Sign.Settings.Presentation.DeliveryMethods;
using Nexticz.Module.Sign.Settings.Presentation.DepositorGroups;
using Nexticz.Module.Sign.Settings.Presentation.Depositors;
using Nexticz.Module.Sign.Settings.Presentation.DocumentTemplates;
using Nexticz.Module.Sign.Settings.Presentation.EmailConfigurations;
using Nexticz.Module.Sign.Settings.Presentation.EmailTemplates;
using Nexticz.Module.Sign.Settings.Presentation.Exports;
using Nexticz.Module.Sign.Settings.Presentation.HistoryEvents;
using Nexticz.Module.Sign.Settings.Presentation.Imports;
using Nexticz.Module.Sign.Settings.Presentation.Locations;
using Nexticz.Module.Sign.Settings.Presentation.OpenApiContracts;
using Nexticz.Module.Sign.Settings.Presentation.Partners;
using Nexticz.Module.Sign.Settings.Presentation.Printers;
using Nexticz.Module.Sign.Settings.Presentation.Projections;
using Nexticz.Module.Sign.Settings.Presentation.Receivers;
using Nexticz.Module.Sign.Settings.Presentation.Seeds;
using Nexticz.Module.Sign.Settings.Presentation.SigningDevices;
using Nexticz.Module.Sign.Settings.Presentation.Users;
using Nexticz.Module.Sign.SharedKernel.Security;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Sign.Settings.Presentation;

internal static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapSignDepositorGroupsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.DepositorGroupEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageSettings)])
                )
            )
            .MapDepositorGroupsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignDeliveryMethodsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.DeliveryMethodEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageSettings)])
                )
            )
            .MapDeliveryMethodsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignPartnersEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.PartnerEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageSettings)])
                )
            )
            .MapPartnersEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignLocationsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.LocationEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageSettings)])
                )
            )
            .MapLocationsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignPrintersEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.PrinterEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageSettings)])
                )
            )
            .MapPrintersEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignSigningDevicesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.SigningDeviceEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageSettings)])
                )
            )
            .MapSigningDevicesEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignMapDepositorsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.DepositorEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageSettings)])
                )
            )
            .MapDepositorsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignMapReceiversEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.ReceiverEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageSettings)])
                )
            )
            .MapReceiversEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignMapUsersEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.UserEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageSettings)])
                )
            )
            .MapUsersEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignHistoryEventsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.HistoryEventEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageSettings)])
                )
            )
            .MapHistoryEventsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignEmailConfigurationsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.EmailConfigurationEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageSettings)])
                )
            )
            .MapEmailConfigurationsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignMapOpenApiContractEndpoints(this IEndpointRouteBuilder builder)
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
    
    public static IEndpointRouteBuilder MapSignDocumentTemplatesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.DocumentTemplateEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageSettings)])
                )
            )
            .MapDocumentTemplatesEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignDeveloperOnlyDocumentTemplatesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.DocumentTemplateEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(AuthorizationHelper.Role.Developer)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapDeveloperOnlyDocumentTemplatesEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignImportsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.ImportEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageSettings)])
                )
            )
            .MapImportsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignExportEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.ExportEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageSettings)])
                )
            )
            .MapExportEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignEmailTemplatesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.EmailTemplateEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageSettings)])
                )
            )
            .MapEmailTemplatesEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignDeveloperOnlyEmailTemplatesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.EmailTemplateEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(AuthorizationHelper.Role.Developer)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapDeveloperOnlyEmailTemplatesEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignSeedsEndpoints(this IEndpointRouteBuilder builder)
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
    
    public static IEndpointRouteBuilder MapSignProjectionEndpoints(this IEndpointRouteBuilder builder)
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
    
    public static IEndpointRouteBuilder MapSignDeveloperOnlyConstantsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.ConstantEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(AuthorizationHelper.Role.Developer)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapDeveloperOnlyConstantsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignConstantsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(SettingsEndpoints.ConstantEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageSettings)])
                )
            )
            .MapConstantsEndpoints();

        return builder;
    }
}