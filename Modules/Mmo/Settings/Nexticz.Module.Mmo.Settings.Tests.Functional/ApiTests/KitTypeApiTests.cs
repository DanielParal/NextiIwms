using System.Net;
using Nexticz.Module.Mmo.Settings.Contracts.KitTypes;
using Nexticz.Module.Mmo.Settings.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Errors.Models;

using Shouldly;

namespace Nexticz.Module.Mmo.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class KitTypeApiTests
{
    private readonly SettingsApiFactoryFixture _fixture;
    private readonly HttpClient _client;

    public KitTypeApiTests(SettingsApiFactoryFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Factory.CreateClient();
    }

    [Fact]
    public async Task CreateKitType_ShouldReturnUnauthorized_WhenNotPassingAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitTypeEndpoints.CreateKitType)
                .WithContent(new
                {
                    Code = "KT1",
                    Name = "Kit type 1"
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateKitType_ShouldReturnForbidden_WhenPassingMemberAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitTypeEndpoints.CreateKitType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyMember)
                .WithContent(new
                {
                    Code = "KT2",
                    Name = "KitType 2"
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateKitType_ShouldSucceed_WhenPassingValidData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitTypeEndpoints.CreateKitType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "KT3",
                    Name = "KitType 3"
                })
                .SendAndDeserializeAsync<KitTypeResponse>();

        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get,
                    $"{SettingsEndpoints.KitTypeEndpoints.GetKitTypes}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<KitTypeResponse>();

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.Created);
        createdResponse.responseContent.ShouldNotBeNull();

        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Id.ShouldBe(createdResponse.responseContent!.Id);
        getResponse.responseContent.Code.ShouldBe(createdResponse.responseContent!.Code);
        getResponse.responseContent.Name.ShouldBe(createdResponse.responseContent!.Name);
    }

    [Fact]
    public async Task CreateKitType_ShouldReturnUnprocessableEntity_WhenPassingEmptyCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitTypeEndpoints.CreateKitType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = string.Empty,
                    Name = "KitType 4"
                })
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }
    
    [Fact]
    public async Task CreateKitType_ShouldReturnUnprocessableEntity_WhenPassingEmptyName()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitTypeEndpoints.CreateKitType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = $"Kit-{Guid.NewGuid()}",
                    Name = string.Empty
                })
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }

    [Fact]
    public async Task CreateKitType_ShouldFail_WhenPassingDuplicatedCode()
    {
        const string sameCode = "KT5";
        var firstCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitTypeEndpoints.CreateKitType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = "KitType 5"
                })
                .SendAsync();

        var secondCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitTypeEndpoints.CreateKitType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = "KitType 6"
                })
                .SendAsync();

        secondCreatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetKitTypes_ShouldReturnData_WhenAtLeastOneExists()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitTypeEndpoints.CreateKitType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "KT7",
                    Name = "KitType 7"
                })
                .SendAsync();

        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get,
                    $"{SettingsEndpoints.KitTypeEndpoints.GetKitTypes}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<KitTypeResponse>>();

        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.GetType().ShouldBe(typeof(FilteredResult<KitTypeResponse>));
        getResponse.responseContent.Data.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task DeleteKitType_ShouldSucceed_WhenPassingValidCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitTypeEndpoints.GetKitTypes)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "KT8",
                    Name = "KitType 8"
                })
                .SendAndDeserializeAsync<KitTypeResponse>();

        var getResponseBeforeDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get,
                    $"{SettingsEndpoints.KitTypeEndpoints.GetKitTypes}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<KitTypeResponse>();

        var deleteResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.KitTypeEndpoints.GetKitTypes}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();

        var getResponseAfterDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get,
                    $"{SettingsEndpoints.KitTypeEndpoints.GetKitTypes}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<KitTypeResponse>(ensureSuccessStatusCode: false);

        getResponseBeforeDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponseBeforeDelete.responseContent.ShouldNotBeNull();

        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        getResponseAfterDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteKitType_ShouldReturnNotFound_WhenKitTypeCodeDoesNotExist()
    {
        var deletedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.KitTypeEndpoints.GetKitTypes}/{Guid.NewGuid()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        deletedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteKitType_ShouldReturnUnprocessableEntity_WhenKitTypeCodeIsUsedInExistingKit()
    {
        var deletedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.KitTypeEndpoints.GetKitTypes}/{_fixture.ApiSeedData.KitTypeCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();

        var contentErrorMessage = await deletedResponse.Content.ReadAsStringAsync();
        
        deletedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        contentErrorMessage.ShouldContain(_fixture.ApiSeedData.KitCode);
    }

    [Fact]
    public async Task UpdateKitType_ShouldSucceed_WhenPassingValidData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitTypeEndpoints.CreateKitType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "KT9",
                    Name = "KitType 9"
                })
                .SendAndDeserializeAsync<KitTypeResponse>();

        const string updatedName = "KitType 10";
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,
                    $"{SettingsEndpoints.KitTypeEndpoints.GetKitTypes}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Name = updatedName
                })
                .SendAsync();

        var getResponseAfterUpdate = await
            new HttpRequestBuilder(_client, HttpMethod.Get,
                    $"{SettingsEndpoints.KitTypeEndpoints.GetKitTypes}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<KitTypeResponse>();

        getResponseAfterUpdate.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponseAfterUpdate.responseContent.ShouldNotBeNull();
        getResponseAfterUpdate.responseContent.Name.ShouldBe(updatedName);
    }
    
    [Fact]
    public async Task UpdateKitType_ShouldReturnUnprocessableEntity_WhenPassingEmptyName()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitTypeEndpoints.CreateKitType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = $"KT-{Guid.NewGuid()}",
                    Name = "KitType 9"
                })
                .SendAndDeserializeAsync<KitTypeResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,
                    $"{SettingsEndpoints.KitTypeEndpoints.GetKitTypes}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Name = string.Empty
                })
                .SendAsync();

        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task UpdateKitType_ShouldReturnNotFound_WhenPassingNonExistingId()
    {
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,
                    $"{SettingsEndpoints.KitTypeEndpoints.GetKitTypes}/{Guid.NewGuid().ToString()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Name = "KitType 11"
                })
                .SendAsync();

        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}