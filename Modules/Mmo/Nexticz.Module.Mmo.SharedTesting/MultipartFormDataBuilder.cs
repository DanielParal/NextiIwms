using System.Net.Http.Headers;

namespace Nexticz.Module.Mmo.SharedTesting;

public class MultipartFormDataBuilder
{
    private readonly MultipartFormDataContent _multipartFormDataContent = new();

    public MultipartFormDataBuilder WithStringData(string name, string content)
    {
        _multipartFormDataContent.Add(new StringContent(content), name);
        return this;
    }
    
    public async Task<MultipartFormDataBuilder> WithFileAsync(string fileName, FileType fileType, string name = "file")
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", fileName);

        var fileBytes = await File.ReadAllBytesAsync(filePath);
        var fileStream = new MemoryStream(fileBytes);
        var fileContent = new StreamContent(fileStream)
        {
            // MIME type for .xlsx files
            Headers = { ContentType = GetContentType(fileType) } 
        };
        _multipartFormDataContent.Add(fileContent, name, fileName); 

        return this;
    }
    
    public MultipartFormDataContent Build()
    {
        return _multipartFormDataContent;
    }
    
    private static MediaTypeHeaderValue GetContentType(FileType fileType)
    {
        return fileType switch
        {
            FileType.Xlsx => new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
            FileType.Txt => new MediaTypeHeaderValue("text/plain"),
            _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
        };
    }
}