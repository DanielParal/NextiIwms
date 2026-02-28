using Microsoft.AspNetCore.Http;

namespace Nexticz.Lib.Shared.DeploymentInfo;

internal class DeploymentInfoHeaderMiddleware(RequestDelegate next, DeploymentInfoSettings deploymentInfoSettings)
{
    private const string DeploymentTimestampHeaderName = "X-Deployment-Timestamp";
    
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[DeploymentTimestampHeaderName] =
                deploymentInfoSettings.LastDeploymentTimestamp.ToString("yyyy-MM-ddTHH:mm:ssZ");;
            return Task.CompletedTask;
        });

        await next(context);
    }
}