using System.ComponentModel.DataAnnotations;

namespace Nexticz.Lib.Shared.FileHandling.Models;

public class FileResponse(string contentBase64, string contentType, string fileName)
{
    [property: Required]
    public string ContentBase64 { get; private set; } = contentBase64;
    
    [property: Required]
    public string ContentType { get; private set; } = contentType;
    
    [property: Required]
    public string FileName { get; private set; } = fileName;

    public FileResponse(byte[] contentBytes, string contentType, string fileName) 
        : this(Convert.ToBase64String(contentBytes), contentType, fileName)
    {
    }
}