using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Nexticz.Lib.Shared.Time;

internal class TenantTimeZoneMiddleware(
    RequestDelegate next,
    IOptions<TimeZoneSettings> timeZoneSettings)
{
    private const string TenantTimeZoneHeaderName = "X-Tenant-TimeZone";

    public async Task Invoke(HttpContext context)
    {
        try
        {
            context.Response.Headers[TenantTimeZoneHeaderName] = timeZoneSettings.Value.TenantTimeZoneId;
        }
        catch
        {
            // ignored
        }
        
        await next(context);
    }
}