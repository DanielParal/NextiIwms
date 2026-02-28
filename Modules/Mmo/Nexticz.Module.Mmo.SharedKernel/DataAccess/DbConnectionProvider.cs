using Microsoft.Extensions.Configuration;

namespace Nexticz.Module.Mmo.SharedKernel.DataAccess;

public static class DbConnectionProvider
{
    public static string GetConnectionString(IConfiguration configuration) => configuration.GetConnectionString("Mmo") ??
                                             throw new InvalidOperationException("Mmo PostgresDb connection string not found.");
}