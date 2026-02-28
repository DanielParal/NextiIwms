using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Lib.Shared.Extensions;


namespace Nexticz.Lib.Shared.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var option = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        try
        {
            await _next(context);
        }
        catch (BadHttpRequestException ex)
        {
            _logger.LogError(ex, "{BadRequestException} was thrown with message: {ErrorMessage}", nameof(BadHttpRequestException), ex.Message);
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var result =
                JsonSerializer.Serialize(Errors.Errors.Common.BadRequest.MapToErrorResponse(), option);

            await context.Response.WriteAsync(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Exception} was thrown with message: {ErrorMessage}", nameof(Exception), ex.Message);
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result =
                JsonSerializer.Serialize(Errors.Errors.Common.InternalServerError.MapToErrorResponse(),
                    option);

            await context.Response.WriteAsync(result);
        }
    }
}