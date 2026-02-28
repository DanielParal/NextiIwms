namespace Nexticz.Module.Mmo.SharedKernel.FileHandling;

public record FileResult : Lib.Shared.FileHandling.Models.FileResult
{
    public FileResult(byte[] contentBytes, string contentType, string fileName) : base(contentBytes, contentType, fileName)
    {
    }
}