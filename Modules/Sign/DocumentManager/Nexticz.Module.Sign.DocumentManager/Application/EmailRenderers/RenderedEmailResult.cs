namespace Nexticz.Module.Sign.DocumentManager.Application.EmailRenderers;

internal record RenderedEmailResult(string Subject, string HtmlBody, string TextBody);