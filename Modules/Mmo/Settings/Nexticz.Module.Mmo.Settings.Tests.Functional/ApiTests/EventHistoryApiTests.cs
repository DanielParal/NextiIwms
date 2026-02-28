using System.Net;
using Nexticz.Module.Mmo.Settings.Contracts.Depositors;
using Nexticz.Module.Mmo.Settings.Contracts.HistoryEvents;
using Nexticz.Module.Mmo.Settings.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Lib.Shared.DevExtreme;

using Shouldly;

namespace Nexticz.Module.Mmo.Settings.Tests.Functional.ApiTests;

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
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.DepositorEndpoints.GetDepositors)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = Guid.NewGuid(),
                    Name = "Depositor 3"
                })
                .SendAndDeserializeAsync<DepositorResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.DepositorEndpoints.GetDepositors}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Name = "Depositor 4"
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