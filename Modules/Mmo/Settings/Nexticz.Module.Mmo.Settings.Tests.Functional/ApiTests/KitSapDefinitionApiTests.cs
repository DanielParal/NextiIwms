using System.Net;
using Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions;
using Nexticz.Module.Mmo.Settings.Contracts.KitSapDefinitions;
using Nexticz.Module.Mmo.Settings.Contracts.KitTypes;
using Nexticz.Module.Mmo.Settings.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Errors.Models;

using Shouldly;

namespace Nexticz.Module.Mmo.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class KitSapDefinitionApiTests
{
    private readonly SettingsApiFactoryFixture _fixture;
    private readonly HttpClient _client;

    public KitSapDefinitionApiTests(SettingsApiFactoryFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Factory.CreateClient();
    }

    [Fact]
    public async Task CreateKitSapDefinition_ShouldReturnUnauthorized_WhenNotPassingAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitSapDefinitionEndpoints.CreateKitSapDefinition)
                .WithContent(new
                {
                    Code = "KT_03",
                    Name = "KT 03"
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateKitSapDefinition_ShouldReturnForbidden_WhenPassingMemberAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitSapDefinitionEndpoints.CreateKitSapDefinition)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyMember)
                .WithContent(new
                {
                    Code = "KT_03",
                    Name = "KT 03"
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateKitSapDefinition_ShouldSucceed_WhenPassingValidData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitSapDefinitionEndpoints.CreateKitSapDefinition)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "KT_03",
                    Name = "KT 03"
                })
                .SendAndDeserializeAsync<KitSapDefinitionResponse>();

        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get,
                    $"{SettingsEndpoints.KitSapDefinitionEndpoints.GetKitSapDefinitions}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<KitSapDefinitionResponse>();

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.Created);
        createdResponse.responseContent.ShouldNotBeNull();

        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Id.ShouldBe(createdResponse.responseContent!.Id);
        getResponse.responseContent.Code.ShouldBe(createdResponse.responseContent!.Code);
        getResponse.responseContent.Name.ShouldBe(createdResponse.responseContent!.Name);
    }

    [Fact]
    public async Task CreateKitSapDefinition_ShouldReturnUnprocessableEntity_WhenPassingEmptyCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitSapDefinitionEndpoints.CreateKitSapDefinition)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = string.Empty,
                    Name = "KT 04"
                })
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }
    
    [Fact]
    public async Task CreateKitSapDefinition_ShouldReturnUnprocessableEntity_WhenPassingEmptyName()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitSapDefinitionEndpoints.CreateKitSapDefinition)
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
    public async Task CreateKitSapDefinition_ShouldFail_WhenPassingDuplicatedCode()
    {
        const string sameCode = "KT_06";
        var firstCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitSapDefinitionEndpoints.CreateKitSapDefinition)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = "KT 06"
                })
                .SendAsync();

        var secondCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitSapDefinitionEndpoints.CreateKitSapDefinition)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = "KT 07"
                })
                .SendAsync();

        secondCreatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetKitSapDefinitions_ShouldReturnData_WhenAtLeastOneExists()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitSapDefinitionEndpoints.CreateKitSapDefinition)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "KT_07",
                    Name = "KT 07"
                })
                .SendAsync();

        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get,
                    $"{SettingsEndpoints.KitSapDefinitionEndpoints.GetKitSapDefinitions}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<KitSapDefinitionResponse>>();

        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.GetType().ShouldBe(typeof(FilteredResult<KitSapDefinitionResponse>));
        getResponse.responseContent.Data.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task DeleteKitSapDefinition_ShouldSucceed_WhenPassingValidCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitSapDefinitionEndpoints.CreateKitSapDefinition)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "KT_08",
                    Name = "KT 08"
                })
                .SendAndDeserializeAsync<KitSapDefinitionResponse>();

        var getResponseBeforeDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get,
                    $"{SettingsEndpoints.KitSapDefinitionEndpoints.GetKitSapDefinitions}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<KitSapDefinitionResponse>();

        var deleteResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.KitSapDefinitionEndpoints.GetKitSapDefinitions}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();

        var getResponseAfterDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get,
                    $"{SettingsEndpoints.KitSapDefinitionEndpoints.GetKitSapDefinitions}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<KitSapDefinitionResponse>(ensureSuccessStatusCode: false);

        getResponseBeforeDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponseBeforeDelete.responseContent.ShouldNotBeNull();

        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        getResponseAfterDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteKitSapDefinition_ShouldReturnNotFound_WhenKitTypeCodeDoesNotExist()
    {
        var deletedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.KitSapDefinitionEndpoints.GetKitSapDefinitions}/{Guid.NewGuid()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        deletedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteKitSapDefinition_ShouldReturnUnprocessableEntity_WhenKitTypeCodeIsUsedInExistingKit()
    {
        var deletedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.KitSapDefinitionEndpoints.GetKitSapDefinitions}/{_fixture.ApiSeedData.KitSapDefinitionCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();

        var contentErrorMessage = await deletedResponse.Content.ReadAsStringAsync();
        
        deletedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        contentErrorMessage.ShouldContain(_fixture.ApiSeedData.KitCode);
        contentErrorMessage.ShouldContain(KitSapDefinitionErrors.ValidationCodeIsStillUsedInKits(_fixture.ApiSeedData.KitCode).Code);
    }

    [Fact]
    public async Task UpdateKitSapDefinition_ShouldSucceed_WhenPassingValidData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitSapDefinitionEndpoints.CreateKitSapDefinition)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "KT_09",
                    Name = "KT 09"
                })
                .SendAndDeserializeAsync<KitSapDefinitionResponse>();

        const string updatedName = "KT 10";
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,
                    $"{SettingsEndpoints.KitSapDefinitionEndpoints.GetKitSapDefinitions}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Name = updatedName
                })
                .SendAsync();

        var getResponseAfterUpdate = await
            new HttpRequestBuilder(_client, HttpMethod.Get,
                    $"{SettingsEndpoints.KitSapDefinitionEndpoints.GetKitSapDefinitions}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<KitSapDefinitionResponse>();

        getResponseAfterUpdate.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponseAfterUpdate.responseContent.ShouldNotBeNull();
        getResponseAfterUpdate.responseContent.Name.ShouldBe(updatedName);
    }
    
    [Fact]
    public async Task UpdateKitSapDefinition_ShouldReturnUnprocessableEntity_WhenPassingEmptyName()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitSapDefinitionEndpoints.CreateKitSapDefinition)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = $"KT-{Guid.NewGuid()}",
                    Name = "KT 11"
                })
                .SendAndDeserializeAsync<KitTypeResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,
                    $"{SettingsEndpoints.KitSapDefinitionEndpoints.GetKitSapDefinitions}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Name = string.Empty
                })
                .SendAsync();

        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task UpdateKitSapDefinition_ShouldReturnNotFound_WhenPassingNonExistingId()
    {
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,
                    $"{SettingsEndpoints.KitSapDefinitionEndpoints.GetKitSapDefinitions}/{Guid.NewGuid().ToString()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Name = "KT 11"
                })
                .SendAsync();

        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}