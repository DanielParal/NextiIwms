using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.DocumentManager.Presentation.Emails;
using Nexticz.Module.Sign.DocumentManager.Presentation.LoadingDocuments;
using Nexticz.Module.Sign.DocumentManager.Presentation.Projections;
using Nexticz.Module.Sign.DocumentManager.Presentation.SignedLoadingDocuments;
using Nexticz.Module.Sign.DocumentManager.Presentation.SigningDevices;
using Nexticz.Module.Sign.DocumentManager.Presentation.UnsignedLoadingDocuments;
using Nexticz.Module.Sign.SharedKernel.Security;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Sign.DocumentManager.Presentation;

internal static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapSignUnsignedLoadingDocumentsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(DocumentManagerEndpoints.UnsignedLoadingDocumentEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageDocumentManager)])
                )
            )
            .MapUnsignedLoadingDocumentsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignSignedLoadingDocumentsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(DocumentManagerEndpoints.SignedLoadingDocumentEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageDocumentManager)])
                )
            )
            .MapSignedLoadingDocumentsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignMapLoadingDocumentsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(DocumentManagerEndpoints.LoadingDocumentEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageDocumentManager)])
                )
            )
            .MapLoadingDocumentsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignDeleteLoadingDocumentsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(DocumentManagerEndpoints.LoadingDocumentEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageDeleteDocuments)])
                )
            )
            .MapDeleteLoadingDocumentsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignSharedSigningDevicesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(DocumentManagerEndpoints.SigningDeviceEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageDocumentManager), nameof(Permission.SignDeviceManageDocuments)])
                )
            )
            .MapSharedSigningDevicesEndpoints();

        return builder;
    }
        
    public static IEndpointRouteBuilder MapSignSigningDevicesForUserEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(DocumentManagerEndpoints.SigningDeviceEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageDocumentManager)])
                )
            )
            .MapSigningDevicesForUserEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignSigningDevicesForDeviceEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(DocumentManagerEndpoints.SigningDeviceEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignDeviceManageDocuments)])
                )
            )
            .MapSigningDevicesForDeviceEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapSignProjectionEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(DocumentManagerEndpoints.ProjectionEndpoints))
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
    
    public static IEndpointRouteBuilder MapSignEmailsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(DocumentManagerEndpoints.EmailEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(Role.SignMember)],
                    [nameof(Permission.SignManageAll), nameof(Permission.SignManageDocumentManager)])
                )
            )
            .MapEmailsEndpoints();

        return builder;
    }
}