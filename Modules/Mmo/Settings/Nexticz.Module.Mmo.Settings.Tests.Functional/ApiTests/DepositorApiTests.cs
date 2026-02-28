using System.Net;
using System.Net.Http.Json;
using Nexticz.Module.Mmo.Settings.Contracts.Depositors;
using Nexticz.Module.Mmo.Settings.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Lib.Shared.DevExtreme;

using Shouldly;

namespace Nexticz.Module.Mmo.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class DepositorApiTests
{
    private readonly SettingsApiFactoryFixture _fixture;
    private readonly HttpClient _client;

    public DepositorApiTests(SettingsApiFactoryFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Factory.CreateClient();
    }
    
    [Fact]
    public async Task GetDepositors_ShouldReturnSomething_WhenAtLeastOneDepositorCreated()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.DepositorEndpoints.CreateDepositor)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "Dep1",
                    Name = "Depositor 1",
                    BarcodeTemplate = "*{{KitNumber}}401KOMPLETY*"
                })
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.DepositorEndpoints.GetDepositors}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<DepositorResponse>>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.GetType().ShouldBe(typeof(FilteredResult<DepositorResponse>));
        getResponse.responseContent.Data.Count.ShouldBeGreaterThan(0);
    }
    
    [Fact]
    public async Task GetDepositors_ShouldReturnUnauthorized_WhenNoTokenProvided()
    {
        var response = await
            new HttpRequestBuilder(_client, HttpMethod.Get, SettingsEndpoints.DepositorEndpoints.GetDepositors)
                .SendAsync();
        
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task GetDepositor_ShouldReturnNotFound_WhenNonExistingDepositorId()
    {
        var response = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.DepositorEndpoints.GetDepositors}/{Guid.NewGuid().ToString()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task CreateDepositor_ShouldReturnCreatedDepositor_WhenOneDepositorCreated()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.DepositorEndpoints.CreateDepositor)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "dep987",
                    Name = "Depositor 1",
                    BarcodeTemplate = "*{{KitNumber}}401KOMPLETY*"
                })
                .SendAndDeserializeAsync<DepositorResponse>();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.DepositorEndpoints.GetDepositors}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<DepositorResponse>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Id.ShouldBe(createdResponse.responseContent.Id);
        getResponse.responseContent.Code.ShouldBe(createdResponse.responseContent.Code);
        getResponse.responseContent.Name.ShouldBe(createdResponse.responseContent.Name);
    }
    
    [Fact]
    public async Task CreateDepositor_ShouldSecondFail_WhenPassingSameCodeTwice()
    {
        const string sameCode = "dp1";
        var firstCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.DepositorEndpoints.CreateDepositor)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = "Depositor 1",
                    BarcodeTemplate = "*{{KitNumber}}401KOMPLETY*"
                })
                .SendAsync();
        
        var secondCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.DepositorEndpoints.CreateDepositor)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = "Depositor 1",
                    BarcodeTemplate = "*{{KitNumber}}401KOMPLETY*"
                })
                .SendAsync();
        
        secondCreatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateDepositor_ShouldReturnUnprocessableEntity_WhenPassingEmptyCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.DepositorEndpoints.CreateDepositor)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = string.Empty,
                    Name = "Depositor 2",
                    BarcodeTemplate = "*{{KitNumber}}401KOMPLETY*"
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateDepositor_ShouldReturnUnprocessableEntity_WhenPassingEmptyName()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.DepositorEndpoints.CreateDepositor)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = $"Dep-{Guid.NewGuid()}",
                    Name = string.Empty,
                    BarcodeTemplate = "*{{KitNumber}}401KOMPLETY*"
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task UpdateDepositor_ShouldSucceed_WhenPassingValidData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.DepositorEndpoints.GetDepositors)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "dp3",
                    Name = "Depositor 3",
                    BarcodeTemplate = "*{{KitNumber}}401KOMPLETY*"
                })
                .SendAndDeserializeAsync<DepositorResponse>();

        var updateBody = new
        {
            Name = "Depositor 4",
            BarcodeTemplate = "*{{KitNumber}}401KOMPLETY*"
        };
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.DepositorEndpoints.GetDepositors}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateBody)
                .SendAsync();
        
        var getResponseAfterUpdate = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.DepositorEndpoints.GetDepositors}/{createdResponse.responseContent.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<DepositorResponse>();
        
        getResponseAfterUpdate.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponseAfterUpdate.responseContent.ShouldNotBeNull();
        getResponseAfterUpdate.responseContent.Name.ShouldBe(updateBody.Name);
    }
    
    [Fact]
    public async Task UpdateManufacture_ShouldReturnNotFound_WhenPassingNonExistingId()
    {
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.DepositorEndpoints.GetDepositors}/{Guid.NewGuid().ToString()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Name = "Depositor 15",
                    BarcodeTemplate = "*{{KitNumber}}401KOMPLETY*"
                })
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteDepositor_ShouldSucceed_WhenPassingValidCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.DepositorEndpoints.GetDepositors)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "dp6",
                    Name = "Depositor 6",
                    BarcodeTemplate = "*{{KitNumber}}401KOMPLETY*"
                })
                .SendAndDeserializeAsync<DepositorResponse>();
        
        var getResponseBeforeDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.DepositorEndpoints.GetDepositors}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<DepositorResponse>();
        
        var deleteResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.DepositorEndpoints.GetDepositors}/{createdResponse.responseContent.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        var getResponseAfterDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.DepositorEndpoints.GetDepositors}/{createdResponse.responseContent.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<DepositorResponse>(ensureSuccessStatusCode: false);
        
        getResponseBeforeDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponseBeforeDelete.responseContent.ShouldNotBeNull();
        
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        
        getResponseAfterDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteDepositor_ShouldReturnNotFound_WhenDepositorCodeDoesNotExist()
    {
        var deletedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.DepositorEndpoints.GetDepositors}/{Guid.NewGuid().ToString()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        deletedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteDepositor_ShouldReturnUnprocessableEntity_WhenDepositorCodeIsUsedEitherInKitOrInPackaging()
    {
        var deletedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.DepositorEndpoints.GetDepositors}/{_fixture.ApiSeedData.DepositorCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();

        var contentErrorMessage = await deletedResponse.Content.ReadAsStringAsync();
        
        deletedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        contentErrorMessage.ShouldContain(_fixture.ApiSeedData.KitCode);
        contentErrorMessage.ShouldContain(_fixture.ApiSeedData.PackagingCode);
    }
}