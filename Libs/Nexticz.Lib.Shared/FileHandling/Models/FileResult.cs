namespace Nexticz.Lib.Shared.FileHandling.Models;

public record FileResult
{
    public byte[] ContentBytes { get; }
    public string ContentType { get; }
    public string FileName { get; }
    public long Length => ContentBytes.Length;

    public FileResult(byte[] contentBytes, string contentType, string fileName)
    {
        ContentBytes = contentBytes ?? throw new ArgumentNullException(nameof(contentBytes));
        ContentType = string.IsNullOrWhiteSpace(contentType) ? throw new ArgumentException("ContentType cannot be null or empty.", nameof(contentType)) : contentType;
        FileName = string.IsNullOrWhiteSpace(fileName) ? throw new ArgumentException("FileName cannot be null or empty.", nameof(fileName)) : fileName;
    }
}