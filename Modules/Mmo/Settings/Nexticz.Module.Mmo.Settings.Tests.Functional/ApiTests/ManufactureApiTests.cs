using System.Net;
using Nexticz.Module.Mmo.Settings.Contracts.Manufactures;
using Nexticz.Module.Mmo.Settings.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Errors.Models;

using Shouldly;

namespace Nexticz.Module.Mmo.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class ManufactureApiTests
{
    private readonly SettingsApiFactoryFixture _fixture;
    private readonly HttpClient _client;

    public ManufactureApiTests(SettingsApiFactoryFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Factory.CreateClient();
    }
    
    [Fact]
    public async Task CreateManufacture_ShouldReturnUnauthorized_WhenNotPassingAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ManufactureEndpoints.CreateManufacture)
                .WithContent(new
                {
                    Code = "Man1",
                    Name = "Manufacture 1"
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task CreateManufacture_ShouldReturnForbidden_WhenPassingMemberAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ManufactureEndpoints.CreateManufacture)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyMember)
                .WithContent(new
                {
                    Code = "Man2",
                    Name = "Manufacture 2"
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task CreateManufacture_ShouldSucceed_WhenPassingValidData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ManufactureEndpoints.CreateManufacture)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "Man3",
                    Name = "Manufacture 3"
                })
                .SendAndDeserializeAsync<ManufactureResponse>();

        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get,
                    $"{SettingsEndpoints.ManufactureEndpoints.GetManufactures}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ManufactureResponse>();

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.Created);
        createdResponse.responseContent.ShouldNotBeNull();

        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Id.ShouldBe(createdResponse.responseContent!.Id);
        getResponse.responseContent.Code.ShouldBe(createdResponse.responseContent!.Code);
        getResponse.responseContent.Name.ShouldBe(createdResponse.responseContent!.Name);
    }
    
    [Fact]
    public async Task CreateManufacture_ShouldFail_WhenPassingEmptyCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ManufactureEndpoints.CreateManufacture)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = string.Empty,
                    Name = "Manufacture 4587"
                })
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }
    
    [Fact]
    public async Task CreateManufacture_ShouldFail_WhenPassingEmptyName()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ManufactureEndpoints.CreateManufacture)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = $"Man-{Guid.NewGuid()}",
                    Name = string.Empty
                })
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }
    
    [Fact]
    public async Task CreateManufacture_ShouldSecondFail_WhenPassingSameManufactureCode()
    {
        const string sameCode = "Man4";
        var firstCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ManufactureEndpoints.CreateManufacture)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = "Manufacture 4"
                })
                .SendAsync();
        
        var secondCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ManufactureEndpoints.CreateManufacture)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = "Manufacture 5"
                })
                .SendAsync();
        
        secondCreatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task GetManufactures_ShouldReturnSomething_WhenAtLeastOneManufactureCreated()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ManufactureEndpoints.CreateManufacture)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "Man5",
                    Name = "Manufacture 5"
                })
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.ManufactureEndpoints.GetManufactures}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<ManufactureResponse>>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.GetType().ShouldBe(typeof(FilteredResult<ManufactureResponse>));
        getResponse.responseContent.Data.Count.ShouldBeGreaterThan(0);
    }
    
    [Fact]
    public async Task DeleteManufacture_ShouldSucceed_WhenPassingValidCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ManufactureEndpoints.CreateManufacture)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "Man6",
                    Name = "Manufacture 6"
                })
                .SendAndDeserializeAsync<ManufactureResponse>();
        
        var getResponseBeforeDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.ManufactureEndpoints.GetManufactures}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ManufactureResponse>();
        
        var deleteResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete, 
                    $"{SettingsEndpoints.ManufactureEndpoints.GetManufactures}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        var getResponseAfterDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.ManufactureEndpoints.GetManufactures}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ManufactureResponse>(ensureSuccessStatusCode: false);
        
        getResponseBeforeDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponseBeforeDelete.responseContent.ShouldNotBeNull();
        
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        
        getResponseAfterDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteManufacture_ShouldReturnNotFound_WhenManufactureCodeDoesNotExist()
    {
        var deletedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.ManufactureEndpoints.GetManufactures}/{Guid.NewGuid()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        deletedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteManufacture_ShouldReturnUnprocessableEntity_WhenManufactureCodeIsUsedInExistingKit()
    {
        var deletedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.ManufactureEndpoints.GetManufactures}/{_fixture.ApiSeedData.ManufactureCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();

        var contentErrorMessage = await deletedResponse.Content.ReadAsStringAsync();
        
        deletedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        contentErrorMessage.ShouldContain(_fixture.ApiSeedData.KitCode);
    }
    
    [Fact]
    public async Task UpdateManufacture_ShouldSucceed_WhenPassingValidData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ManufactureEndpoints.CreateManufacture)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "Man7",
                    Name = "Manufacture 7"
                })
                .SendAndDeserializeAsync<ManufactureResponse>();
        
        const string updatedName = "Manufacture 8";
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put, 
                    $"{SettingsEndpoints.ManufactureEndpoints.GetManufactures}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Name = updatedName
                })
                .SendAsync();
        
        var getResponseAfterUpdate = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.ManufactureEndpoints.GetManufactures}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ManufactureResponse>();
        
        getResponseAfterUpdate.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponseAfterUpdate.responseContent.ShouldNotBeNull();
        getResponseAfterUpdate.responseContent.Name.ShouldBe(updatedName);
    }
    
    [Fact]
    public async Task UpdateManufacture_ShouldReturnUnprocessableEntity_WhenPassingEmptyName()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ManufactureEndpoints.CreateManufacture)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = $"Man-{Guid.NewGuid()}",
                    Name = "Manufacture 7"
                })
                .SendAndDeserializeAsync<ManufactureResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put, 
                    $"{SettingsEndpoints.ManufactureEndpoints.GetManufactures}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Name = string.Empty
                })
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task UpdateManufacture_ShouldReturnNotFound_WhenPassingNonExistingId()
    {
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put, 
                    $"{SettingsEndpoints.ManufactureEndpoints.GetManufactures}/{Guid.NewGuid().ToString()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Name = "Manufacture 10"
                })
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}