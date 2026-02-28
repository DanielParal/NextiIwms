using System.Net;
using Nexticz.Module.Mmo.Settings.Contracts.Manufactures;
using Nexticz.Module.Mmo.Settings.Contracts.PackagingTypes;
using Nexticz.Module.Mmo.Settings.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Lib.Shared.DevExtreme;

using Shouldly;

namespace Nexticz.Module.Mmo.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class PackagingTypeApiTests
{
    private readonly SettingsApiFactoryFixture _fixture;
    private readonly HttpClient _client;

    public PackagingTypeApiTests(SettingsApiFactoryFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Factory.CreateClient();
    }

    [Fact]
    public async Task CreatePackagingType_ShouldReturnUnauthorized_WhenNotPassingAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingTypeEndpoints.CreatePackagingType)
                .WithContent(new
                {
                    Code = "PT1",
                    Name = "Packaging type 1"
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task CreatePackagingType_ShouldReturnForbidden_WhenNotPassingAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingTypeEndpoints.CreatePackagingType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyMember)
                .WithContent(new
                {
                    Code = "PT2",
                    Name = "Packaging type 2"
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task CreatePackagingType_ShouldReturnCreatedPackagingType_WhenOneDepositorCreated()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.PackagingTypeEndpoints.CreatePackagingType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "PT3",
                    Name = "Packaging type 3"
                })
                .SendAndDeserializeAsync<PackagingTypeResponse>();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.PackagingTypeEndpoints.GetPackagingTypes}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<PackagingTypeResponse>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Id.ShouldBe(createdResponse.responseContent.Id);
        getResponse.responseContent.Code.ShouldBe(createdResponse.responseContent.Code);
        getResponse.responseContent.Name.ShouldBe(createdResponse.responseContent.Name);
    }
    
    [Fact]
    public async Task CreatePackagingType_ShouldReturnUnprocessableEntity_WhenPassingEmptyCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.PackagingTypeEndpoints.CreatePackagingType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = string.Empty,
                    Name = "Packaging type 3"
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreatePackagingType_ShouldReturnUnprocessableEntity_WhenPassingEmptyName()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.PackagingTypeEndpoints.CreatePackagingType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = $"PT-{Guid.NewGuid()}",
                    Name = string.Empty
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreatePackagingType_ShouldSecondFail_WhenPassingSameCode()
    {
        const string sameCode = "PT4";
        var firstCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingTypeEndpoints.CreatePackagingType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = "Packaging type 4"
                })
                .SendAsync();
        
        var secondCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingTypeEndpoints.CreatePackagingType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = "Packaging type 5"
                })
                .SendAsync();
        
        secondCreatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task GetPackagingTypes_ShouldReturnSomething_WhenAtLeastOnePackagingTypeCreated()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingTypeEndpoints.CreatePackagingType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "PT5",
                    Name = "Packaging type 5"
                })
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.PackagingTypeEndpoints.CreatePackagingType}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<PackagingTypeResponse>>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.GetType().ShouldBe(typeof(FilteredResult<PackagingTypeResponse>));
        getResponse.responseContent.Data.Count.ShouldBeGreaterThan(0);
    }
    
    [Fact]
    public async Task UpdatePackagingType_ShouldSucceed_WhenPassingValidData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.PackagingTypeEndpoints.CreatePackagingType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "PT6",
                    Name = "Packaging Type 6"
                })
                .SendAndDeserializeAsync<PackagingTypeResponse>();

        const string bodyNameToBeUpdated = "Packaging Type 7";
        await new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.PackagingTypeEndpoints.GetPackagingTypes}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Name = bodyNameToBeUpdated
                })
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.PackagingTypeEndpoints.GetPackagingTypes}/{createdResponse.responseContent.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<PackagingTypeResponse>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Code.ShouldBe(createdResponse.responseContent.Code);
        getResponse.responseContent.Name.ShouldBe(bodyNameToBeUpdated);
    }
    
    [Fact]
    public async Task UpdatePackagingType_ShouldReturnUnprocessableEntity_WhenPassingEmptyName()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.PackagingTypeEndpoints.CreatePackagingType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = $"PT-{Guid.NewGuid()}",
                    Name = "Packaging Type 6"
                })
                .SendAndDeserializeAsync<PackagingTypeResponse>();
        
        var updatedResponse =
            await new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.PackagingTypeEndpoints.GetPackagingTypes}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Name = (string)null
                })
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task UpdatePackagingType_ShouldReturnNotFound_WhenPassingNonExistingId()
    {
        var updatedResponse = await 
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.PackagingTypeEndpoints.GetPackagingTypes}/{Guid.NewGuid().ToString()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Name = "Packaging Type 7"
                })
                .SendAsync();
    
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeletePackagingType_ShouldSucceed_WhenPassingValidPackagingType()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingTypeEndpoints.CreatePackagingType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "PT8",
                    Name = "Packaging Type 8"
                })
                .SendAndDeserializeAsync<PackagingTypeResponse>();
        
        var getResponseBeforeDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.PackagingTypeEndpoints.GetPackagingTypes}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<PackagingTypeResponse>();
        
        var deleteResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete, 
                    $"{SettingsEndpoints.PackagingTypeEndpoints.GetPackagingTypes}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        var getResponseAfterDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.PackagingTypeEndpoints.GetPackagingTypes}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<PackagingTypeResponse>(ensureSuccessStatusCode: false);
        
        getResponseBeforeDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponseBeforeDelete.responseContent.ShouldNotBeNull();
        
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        
        getResponseAfterDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeletePackagingType_ShouldReturnNotFound_WhenPackagingTypeIdNotFound()
    {
        var deletedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.PackagingTypeEndpoints.GetPackagingTypes}/{Guid.NewGuid().ToString()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        deletedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeletePackagingType_ShouldReturnUnprocessableEntity_WhenPackagingTypeCodeIsUsedInExistingPackaging()
    {
        var deletedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.PackagingTypeEndpoints.GetPackagingTypes}/{_fixture.ApiSeedData.PackagingTypeCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        var contentErrorMessage = await deletedResponse.Content.ReadAsStringAsync();
        
        deletedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        contentErrorMessage.ShouldContain(_fixture.ApiSeedData.PackagingCode);
    }
}