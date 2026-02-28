using System.Net;
using Nexticz.Module.Mmo.Settings.Contracts.PackagingCirculations;
using Nexticz.Module.Mmo.Settings.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Errors.Models;

using Shouldly;

namespace Nexticz.Module.Mmo.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class PackagingCirculationApiTests
{
    private readonly SettingsApiFactoryFixture _fixture;
    private readonly HttpClient _client;

    public PackagingCirculationApiTests(SettingsApiFactoryFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Factory.CreateClient();
    }

    [Fact]
    public async Task CreatePackagingCirculation_ShouldReturnUnauthorized_WhenNotPassingAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingCirculationEndpoints.CreatePackagingCirculation)
                .WithContent(new
                {
                    Code = "PacCir1",
                    Name = "PackagingCirculation 1"
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreatePackagingCirculation_ShouldReturnForbidden_WhenPassingMemberAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingCirculationEndpoints.CreatePackagingCirculation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyMember)
                .WithContent(new
                {
                    Code = "PacCir2",
                    Name = "PackagingCirculation 2"
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreatePackagingCirculation_ShouldSucceed_WhenPassingValidData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingCirculationEndpoints.CreatePackagingCirculation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "PacCir3",
                    Name = "PackagingCirculation 3"
                })
                .SendAndDeserializeAsync<PackagingCirculationResponse>();

        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get,
                    $"{SettingsEndpoints.PackagingCirculationEndpoints.GetPackagingCirculations}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<PackagingCirculationResponse>();

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.Created);
        createdResponse.responseContent.ShouldNotBeNull();

        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Id.ShouldBe(createdResponse.responseContent!.Id);
        getResponse.responseContent.Code.ShouldBe(createdResponse.responseContent!.Code);
        getResponse.responseContent.Name.ShouldBe(createdResponse.responseContent!.Name);
    }

    [Fact]
    public async Task CreatePackagingCirculation_ShouldReturnUnprocessableEntity_WhenPassingEmptyCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingCirculationEndpoints.CreatePackagingCirculation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = string.Empty,
                    Name = "PackagingCirculation 4"
                })
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }
    
    [Fact]
    public async Task CreatePackagingCirculation_ShouldReturnUnprocessableEntity_WhenPassingEmptyName()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingCirculationEndpoints.CreatePackagingCirculation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = $"PC-{Guid.NewGuid()}",
                    Name = string.Empty
                })
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }

    [Fact]
    public async Task CreatePackagingCirculation_ShouldFail_WhenPassingDuplicatedCode()
    {
        const string sameCode = "PacCir5";
        var firstCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingCirculationEndpoints.CreatePackagingCirculation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = "PackagingCirculation 5"
                })
                .SendAsync();

        var secondCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingCirculationEndpoints.CreatePackagingCirculation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = "PackagingCirculation 6"
                })
                .SendAsync();

        secondCreatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetPackagingCirculations_ShouldReturnData_WhenAtLeastOneExists()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingCirculationEndpoints.CreatePackagingCirculation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "PacCir7",
                    Name = "PackagingCirculation 7"
                })
                .SendAsync();

        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get,
                    $"{SettingsEndpoints.PackagingCirculationEndpoints.GetPackagingCirculations}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<PackagingCirculationResponse>>();

        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.GetType().ShouldBe(typeof(FilteredResult<PackagingCirculationResponse>));
        getResponse.responseContent.Data.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task DeletePackagingCirculation_ShouldSucceed_WhenPassingValidCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingCirculationEndpoints.CreatePackagingCirculation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "PacCir8",
                    Name = "PackagingCirculation 8"
                })
                .SendAndDeserializeAsync<PackagingCirculationResponse>();

        var getResponseBeforeDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get,
                    $"{SettingsEndpoints.PackagingCirculationEndpoints.GetPackagingCirculations}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<PackagingCirculationResponse>();

        var deleteResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.PackagingCirculationEndpoints.GetPackagingCirculations}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();

        var getResponseAfterDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get,
                    $"{SettingsEndpoints.PackagingCirculationEndpoints.GetPackagingCirculations}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<PackagingCirculationResponse>(ensureSuccessStatusCode: false);

        getResponseBeforeDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponseBeforeDelete.responseContent.ShouldNotBeNull();

        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        getResponseAfterDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeletePackagingCirculation_ShouldReturnNotFound_WhenPackagingCirculationCodeDoesNotExist()
    {
        var deletedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.PackagingCirculationEndpoints.GetPackagingCirculations}/{Guid.NewGuid()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        deletedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeletePackagingCirculation_ShouldReturnUnprocessableEntity_WhenPackagingCirculationCodeIsUsedInExistingPackaging()
    {
        var deletedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.PackagingCirculationEndpoints.GetPackagingCirculations}/{_fixture.ApiSeedData.PackagingCirculationCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();

        var contentErrorMessage = await deletedResponse.Content.ReadAsStringAsync();
        
        deletedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        contentErrorMessage.ShouldContain(_fixture.ApiSeedData.PackagingCode);
    }

    [Fact]
    public async Task UpdatePackagingCirculation_ShouldSucceed_WhenPassingValidData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingCirculationEndpoints.CreatePackagingCirculation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "PacCir9",
                    Name = "PackagingCirculation 9"
                })
                .SendAndDeserializeAsync<PackagingCirculationResponse>();

        const string updatedName = "PackagingCirculation 10";
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,
                    $"{SettingsEndpoints.PackagingCirculationEndpoints.GetPackagingCirculations}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Name = updatedName
                })
                .SendAsync();

        var getResponseAfterUpdate = await
            new HttpRequestBuilder(_client, HttpMethod.Get,
                    $"{SettingsEndpoints.PackagingCirculationEndpoints.GetPackagingCirculations}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<PackagingCirculationResponse>();

        getResponseAfterUpdate.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponseAfterUpdate.responseContent.ShouldNotBeNull();
        getResponseAfterUpdate.responseContent.Name.ShouldBe(updatedName);
    }
    
    [Fact]
    public async Task UpdatePackagingCirculation_ShouldReturnUnprocessableEntity_WhenPassingEmptyName()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingCirculationEndpoints.CreatePackagingCirculation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = $"PC-{Guid.NewGuid()}",
                    Name = "PackagingCirculation 9"
                })
                .SendAndDeserializeAsync<PackagingCirculationResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,
                    $"{SettingsEndpoints.PackagingCirculationEndpoints.GetPackagingCirculations}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Name = string.Empty
                })
                .SendAsync();

        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task UpdatePackagingCirculation_ShouldReturnNotFound_WhenPassingNonExistingId()
    {
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,
                    $"{SettingsEndpoints.PackagingCirculationEndpoints.GetPackagingCirculations}/{Guid.NewGuid().ToString()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Name = "PackagingCirculation 11"
                })
                .SendAsync();

        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}