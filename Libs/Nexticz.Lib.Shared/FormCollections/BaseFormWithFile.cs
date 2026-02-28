using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Nexticz.Lib.Shared.FormCollections;

public abstract class BaseFormWithFile<T>(T request, IFormFile file)
{
    [FromForm(Name = "request")]
    [property: Required]
    public T Request { get; set; } = request;
    
    [FromForm(Name = "file")]
    [property: Required]
    public IFormFile File { get; set; } = file;
    
    public static bool TryGetRequestFromFormCollection(IFormCollection formCollection, out T? result)
    {
        var requestJson = formCollection[nameof(Request)];
        
        if (string.IsNullOrWhiteSpace(requestJson))
        {
            result = default;
            return false;
        }
        
        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true};
            options.Converters.Add(new JsonStringEnumConverter());
            result = JsonSerializer.Deserialize<T>(requestJson!, options);
            return true;
        }
        catch (Exception e)
        {
            result = default;
            return false;
        }
    }
}