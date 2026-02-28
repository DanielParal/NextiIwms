using Microsoft.AspNetCore.Http;

namespace Nexticz.Lib.Shared.ImportsExports.Imports.ImportFiles;

public class FormImportFile(IFormFile file) : IImportFile, IDisposable, IAsyncDisposable
{
    private Stream? _stream;
    
    public string FileName => file.FileName;
    public string? ContentType => file.ContentType;

    public Stream Stream => _stream ??= file.OpenReadStream();

    public void Dispose()
    {
        _stream?.Dispose();
        _stream = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_stream != null)
        {
            await _stream.DisposeAsync();
            _stream = null;
        }
    }
}