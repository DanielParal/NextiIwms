using System.Net;
using Nexticz.Module.Sign.Settings.Application.Locations;
using Nexticz.Module.Sign.Settings.Contracts.Locations;
using Nexticz.Module.Sign.Settings.Presentation;
using Nexticz.Module.Sign.SharedTesting;
using Nexticz.Lib.Shared.Errors.Models;
using Shouldly;

namespace Nexticz.Module.Sign.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class LocationApiTests
{
    private readonly SettingsApiFactoryFixture _fixture;
    private readonly HttpClient _client;

    public LocationApiTests(SettingsApiFactoryFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Factory.CreateClient();
    }
    
    [Fact]
    public async Task HappyPathTestWithAllCrudEndpoints_ShouldCreateUpdateAndDeleteLocation_WhenCallingAllEndpointsOneByOne()
    {
        const string code = "LocationApiTests_LocationCode";
        const string nameWhenCreated = "LocationApiTests_LocationName";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.LocationEndpoints.CreateLocation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = code,
                    Name = nameWhenCreated
                })
                .SendAndDeserializeAsync<LocationResponse>();
        
        await VerifyLocationAsync(createdResponse.responseContent!.Code, nameWhenCreated);
        
        var updateBody = new
        {
            Name = "LocationApiTests_LocationNameUpdated"
        };
        await new HttpRequestBuilder(_client, HttpMethod.Put,
                $"{SettingsEndpoints.LocationEndpoints.GetLocations}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateBody)
                .SendAsync();
        
        await VerifyLocationAsync(createdResponse.responseContent!.Code, updateBody.Name);
        
        await new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.LocationEndpoints.GetLocations}/{createdResponse.responseContent.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();

        await VerifyNotFoundLocationAsync(createdResponse.responseContent!.Code);
    }
    
    [Fact]
    public async Task CreateLocation_ShouldFail_WhenPassingEmptyCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.LocationEndpoints.CreateLocation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = string.Empty,
                    Name = Guid.Empty
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateSameLocations_ShouldSecondFail_WhenPassingSameCodes()
    {
        var sameCode = $"SameCode_{Guid.NewGuid()}";
        await new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.LocationEndpoints.CreateLocation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = Guid.Empty
                })
                .SendAsync();
        
        var secondCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.LocationEndpoints.CreateLocation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = Guid.Empty
                })
                .SendAsync();
        
        secondCreatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task DeleteLocation_ShouldFail_WhenCodeStillUsedInSigningDevice()
    {
        var deletedResponse = await new HttpRequestBuilder(_client, HttpMethod.Delete,
                $"{SettingsEndpoints.LocationEndpoints.GetLocations}/{_fixture.ApiSeedData.LocationCode1}")
            .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
            .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        deletedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        deletedResponse.responseContent!.Errors.First().Slug.ShouldBe(LocationErrors.ValidationCodeIsUsedInSigningDevices(_fixture.ApiSeedData.SigningDeviceCode).Code);
        deletedResponse.responseContent!.Errors.First().Message.ShouldBe(LocationErrors.ValidationCodeIsUsedInSigningDevices(_fixture.ApiSeedData.SigningDeviceCode).Description);
    }

    private async Task VerifyLocationAsync(string code, string expectedName)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.LocationEndpoints.GetLocations}/{code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<LocationResponse>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Code.ShouldBe(code);
        getResponse.responseContent.Name.ShouldBe(expectedName);
    }
    
    private async Task VerifyNotFoundLocationAsync(string code)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.LocationEndpoints.GetLocations}/{code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<LocationResponse>(ensureSuccessStatusCode: false);
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}