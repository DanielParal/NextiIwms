using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Nexticz.Lib.Shared.Helpers;

public static class AuthorizationHelper
{
    public enum Permission
    {
        Any
    }

    public enum Role
    {
        Developer,
        SysAdmin,
        Anonymous, // Anonymous role is used for frontend to access public endpoints - we need to have it here for Open api
        Any
    }

    public static bool HasUserAccessRights(HttpContext context, List<string> roles, List<string> permissions)
    {
        if (HasUserAdminOrFullAccessRights(context, roles, permissions))
            return true;
        
        return !HasEndpointEmptyRolesOrPermissions(roles, permissions) 
               && HasUserRequiredRoleAndPermission(context, roles, permissions);
    }

    public static bool AddRequiredClaimsToHttpContextItems(AuthorizationHandlerContext context, List<string> roles,
        List<string> permissions)
    {
        var httpContext = (HttpContext)context.Resource!;

        httpContext.Items[StringHelper.Claim.Type.MagicRoles] = roles;
        httpContext.Items[StringHelper.Claim.Type.MagicPermissions] = permissions;

        return true;
    }

    private static bool HasUserAdminOrFullAccessRights(HttpContext context, List<string> roles, List<string> permissions)
    {
        string[] fullAccessRoles = [nameof(Role.Developer), nameof(Role.SysAdmin)];
        
        return context.User.Claims.Any(c => c.Type == StringHelper.Claim.Type.MagicRoles && fullAccessRoles.Contains(c.Value))
                || roles.Any(x => x == nameof(Role.Any)) && permissions.Any(x => x == nameof(Permission.Any));
    }

    private static bool HasEndpointEmptyRolesOrPermissions(List<string> roles, List<string> permissions)
    {
        return roles.Count == 0 || permissions.Count == 0;
    }

    private static bool HasUserRequiredRoleAndPermission(HttpContext context, List<string> roles,
        List<string> permissions)
    {
        return context.User.Claims.Any(c =>
                   c.Type == StringHelper.Claim.Type.MagicRoles && roles.Contains(c.Value) ||
                   roles.Contains(nameof(Role.Any)))
               && context.User.Claims.Any(c =>
                   c.Type == StringHelper.Claim.Type.MagicPermissions && permissions.Contains(c.Value) ||
                   permissions.Contains(nameof(Permission.Any)));
    }
}