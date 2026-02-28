using Nexticz.Lib.Shared.Generators;

namespace Nexticz.Lib.Shared.Logging;

public sealed class CorrelationIdProvider
{
    public const string CorrelationIdHeaderName = "X-Correlation-ID";
    
    private const int GeneratedIdLength = 7;
    private static readonly string EmptyCorrelationId = new('0', GeneratedIdLength);

    private readonly AsyncLocal<string> _internalId = new();
    
    private readonly CorrelationIdSanitizer _sanitizer = new(maxCorrelationIdLength: 40);

    private CorrelationIdProvider()
    {
    }

    public static CorrelationIdProvider Instance { get; } = new();

    private string InternalId
    {
        get
        {
            var correlationId = GetInternalId();
            return correlationId.Length > GeneratedIdLength
                ? correlationId.Substring(correlationId.Length - GeneratedIdLength)
                : correlationId;
        }
    }

    /// <summary>
    /// Returns the internal correlation id, generated for current execution context.
    /// </summary>
    /// <returns></returns>
    public string GetInternalId()
    {
        try
        {
            var value = _internalId.Value;
            if (!string.IsNullOrEmpty(value))
                return value;

            value = GenerateId();
            _internalId.Value = value;

            return value;
        }
        catch
        {
            return EmptyCorrelationId;
        }
    }

    public void SetExternalIdAsPrefix(string externalCorrelationId)
    {
        var externalId = _sanitizer.Sanitize(externalCorrelationId);
        if (string.IsNullOrEmpty(externalId))
        {
            _internalId.Value = InternalId;
            return;
        }
        
        _internalId.Value = $"{externalId}_{InternalId}";
    }

    private static string GenerateId() => Base62IdGenerator.GenerateId(GeneratedIdLength);
}