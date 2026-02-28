using Microsoft.Extensions.Configuration;

namespace Nexticz.Lib.Shared.DataAccess;

public static class DbConnectionFactory
{
    public static string CreateMsSql(IConfiguration config)
    {
        return config.GetConnectionString("DefaultConnectionString")
            ??  throw new InvalidOperationException("DefaultConnectionString Settings not found.");
    }
}

