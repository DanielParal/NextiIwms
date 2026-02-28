using System.Net;
using Nexticz.Module.Sign.Settings.Contracts.HistoryEvents;
using Nexticz.Module.Sign.Settings.Contracts.Locations;
using Nexticz.Module.Sign.Settings.Presentation;
using Nexticz.Module.Sign.SharedTesting;
using Nexticz.Lib.Shared.DevExtreme;
using Shouldly;

namespace Nexticz.Module.Sign.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class EventHistoryApiTests
{
    private readonly HttpClient _client;

    public EventHistoryApiTests(SettingsApiFactoryFixture fixture)
    {
        _client = fixture.Factory.CreateClient();
    }
    
    [Fact]
    public async Task GetHistoryEvents_ShouldReturnUnauthorized_WhenNotPassingAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, $"{SettingsEndpoints.HistoryEventEndpoints.Base}/{Guid.NewGuid().ToString()}")
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task GetHistoryEvents_ShouldReturnSomething_WhenEntityHasValues()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.LocationEndpoints.CreateLocation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = Guid.NewGuid(),
                    Name = Guid.NewGuid()
                })
                .SendAndDeserializeAsync<LocationResponse>();
        
        await new HttpRequestBuilder(_client, HttpMethod.Put,
                $"{SettingsEndpoints.LocationEndpoints.GetLocations}/{createdResponse.responseContent!.Code}")
            .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
            .WithContent(new
            {
                Name = Guid.NewGuid()
            })
            .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.HistoryEventEndpoints.Base}/{createdResponse.responseContent!.Id.ToString()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<HistoryEventResponse>>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.GetType().ShouldBe(typeof(FilteredResult<HistoryEventResponse>));
        getResponse.responseContent.Data.Count.ShouldBe(2);
    }
}