using Microsoft.Extensions.Configuration;

namespace Nexticz.Module.Sign.SharedKernel.DataAccess;

public static class DbConnectionProvider
{
    public static string GetConnectionString(IConfiguration configuration) => configuration.GetConnectionString("Sign") ??
                                             throw new InvalidOperationException("Sign PostgresDb connection string not found.");
}