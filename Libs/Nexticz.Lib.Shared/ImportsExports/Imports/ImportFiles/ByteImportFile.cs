namespace Nexticz.Lib.Shared.ImportsExports.Imports.ImportFiles;

public class ByteImportFile(byte[] bytes, string fileName, string contentType) : IImportFile, IDisposable, IAsyncDisposable
{
    private Stream? _stream;
    
    public string FileName => fileName;
    public string? ContentType => contentType;

    public Stream Stream => _stream ??= new MemoryStream(bytes);
    
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