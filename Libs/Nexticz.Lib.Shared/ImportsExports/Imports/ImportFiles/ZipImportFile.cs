using System.IO.Compression;

namespace Nexticz.Lib.Shared.ImportsExports.Imports.ImportFiles;

public class ZipImportFile : IImportFile, IDisposable, IAsyncDisposable
{
    private readonly ZipArchiveEntry _entry;
    public ZipImportFile(ZipArchiveEntry entry)
    {
        _entry = entry;
        Stream = CreateBufferedStream();
    }

    public string FileName => _entry.FullName;
    public string? ContentType => null; // ZIP entries don’t have content type
    public Stream Stream { get; private set; }

    public void Dispose()
    {
        Stream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Stream.DisposeAsync();
    }
    
    private Stream CreateBufferedStream()
    {
        var ms = new MemoryStream();
        using var source = _entry.Open();
        source.CopyTo(ms);
        ms.Position = 0;
        return ms;
    }

}