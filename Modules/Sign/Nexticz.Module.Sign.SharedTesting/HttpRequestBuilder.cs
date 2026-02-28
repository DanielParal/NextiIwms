using System.Net.Http.Json;

namespace Nexticz.Module.Sign.SharedTesting;

public class HttpRequestBuilder(HttpClient client, HttpMethod method, string uri)
{
    private readonly HttpRequestMessage _request = new(method, uri);

    public HttpRequestBuilder WithXApiKeyHeader(string value)
    {
        _request.Headers.Add("X-API-Key", value);
        return this;
    }

    public HttpRequestBuilder WithContent(object content)
    {
        _request.Content = JsonContent.Create(content);
        return this;
    }
    
    public HttpRequestBuilder WithMultipartFormData(MultipartFormDataContent multipartFormDataContent)
    {
        _request.Content = multipartFormDataContent;
        return this;
    }

    public async Task<HttpResponseMessage> SendAsync(CancellationToken cancellationToken = default)
    {
        return await client.SendAsync(_request, cancellationToken);
    }
    
    public async Task<(HttpResponseMessage responseMessage, T? responseContent)> SendAndDeserializeAsync<T>(bool ensureSuccessStatusCode = true, CancellationToken cancellationToken = default)
    {
        var responseMessage = await client.SendAsync(_request, cancellationToken);
        
        if(ensureSuccessStatusCode)
            responseMessage.EnsureSuccessStatusCode();
        
        var responseContent = await responseMessage.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        
        return (responseMessage, responseContent);
    }
}