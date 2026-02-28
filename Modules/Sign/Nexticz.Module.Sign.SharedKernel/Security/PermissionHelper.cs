namespace Nexticz.Module.Sign.SharedKernel.Security;

public static class PermissionHelper
{
    public static string[] GetNames()
    {
        return Enum.GetNames<Permission>();
    }
}