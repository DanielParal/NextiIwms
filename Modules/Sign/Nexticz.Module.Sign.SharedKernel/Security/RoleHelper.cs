namespace Nexticz.Module.Sign.SharedKernel.Security;

public static class RoleHelper 
{
    public static string[] GetNames()
    {
        return Enum.GetNames<Role>();
    }
}