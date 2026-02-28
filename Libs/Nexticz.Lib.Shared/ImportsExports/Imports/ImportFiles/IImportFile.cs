namespace Nexticz.Lib.Shared.ImportsExports.Imports.ImportFiles;

public interface IImportFile
{
    string FileName { get; }
    string? ContentType { get; }
    Stream Stream { get; }

    ValueTask DisposeAsync();
}