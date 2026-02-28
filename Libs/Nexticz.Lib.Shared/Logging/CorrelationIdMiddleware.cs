using Microsoft.AspNetCore.Http;

namespace Nexticz.Lib.Shared.Logging;

internal class CorrelationIdMiddleware
{
    private const string RequestIdHeaderName = "X-Request-ID";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            CorrelationIdProvider.Instance.SetExternalIdAsPrefix(context.Request.Headers[RequestIdHeaderName].FirstOrDefault());
            context.Response.Headers[RequestIdHeaderName] = CorrelationIdProvider.Instance.GetInternalId();
        }
        catch
        {
            // ignored
        }
        
        await _next(context);
    }
}