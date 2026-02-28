namespace Nexticz.Module.Mmo.SharedKernel.Security;

public static class PermissionHelper
{
    public static string[] GetNames()
    {
        return Enum.GetNames<Permission>();
    }
}