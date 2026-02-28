namespace Nexticz.Lib.Shared.Errors.Models;

public class ApiErrorResponse
{
    public required string CorrelationId { get; set; }
    public required List<ApiError> Errors { get; set; }
}

public class ApiError
{
    public required string Slug { get; set; }
    public required string Message { get; set; }
}