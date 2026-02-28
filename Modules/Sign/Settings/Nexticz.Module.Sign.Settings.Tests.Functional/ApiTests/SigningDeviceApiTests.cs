using System.Net;
using Nexticz.Module.Sign.Settings.Application.SigningDevices;
using Nexticz.Module.Sign.Settings.Contracts.SigningDevices;
using Nexticz.Module.Sign.Settings.Presentation;
using Nexticz.Module.Sign.SharedTesting;
using Nexticz.Lib.Shared.Errors.Models;
using Shouldly;

namespace Nexticz.Module.Sign.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class SigningDeviceApiTests
{
    private readonly SettingsApiFactoryFixture _fixture;
    private readonly HttpClient _client;

    public SigningDeviceApiTests(SettingsApiFactoryFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Factory.CreateClient();
    }
    
    [Fact]
    public async Task HappyPathTestWithAllCrudEndpoints_ShouldCreateUpdateAndDeleteSigningDevice_WhenCallingAllEndpointsOneByOne()
    {
        const string code = "SigningDeviceApiTests_SigningDeviceCode";
        const string nameWhenCreated = "SigningDeviceApiTests_SigningDeviceName";
        const bool ipActiveWhenCreated = true;
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.SigningDeviceEndpoints.CreateSigningDevice)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = code,
                    Name = nameWhenCreated,
                    IsActive = ipActiveWhenCreated,
                    LocationCode = _fixture.ApiSeedData.LocationCode1,
                    PrinterCode = _fixture.ApiSeedData.PrinterCode1
                })
                .SendAndDeserializeAsync<SigningDeviceResponse>();
        
        await VerifySigningDeviceAsync(
            createdResponse.responseContent!.Code, nameWhenCreated, ipActiveWhenCreated, 
            _fixture.ApiSeedData.LocationCode1, _fixture.ApiSeedData.PrinterCode1);
        
        var updateBody = new
        {
            Name = "SigningDeviceApiTests_SigningDeviceNameUpdated",
            IsActive = false,
            LocationCode = _fixture.ApiSeedData.LocationCode2,
            PrinterCode = _fixture.ApiSeedData.PrinterCode2     
        };
        await new HttpRequestBuilder(_client, HttpMethod.Put,
                $"{SettingsEndpoints.SigningDeviceEndpoints.GetSigningDevices}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateBody)
                .SendAsync();
        
        await VerifySigningDeviceAsync(createdResponse.responseContent!.Code, updateBody.Name, updateBody.IsActive, updateBody.LocationCode, updateBody.PrinterCode);
        
        await new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.SigningDeviceEndpoints.GetSigningDevices}/{createdResponse.responseContent.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();

        await VerifyNotFoundSigningDeviceAsync(createdResponse.responseContent!.Code);
    }
    
    [Fact]
    public async Task CreateSigningDevice_ShouldFail_WhenPassingEmptyCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.SigningDeviceEndpoints.CreateSigningDevice)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = string.Empty,
                    Name = Guid.NewGuid(),
                    IsActive = true,
                    LocationCode = _fixture.ApiSeedData.LocationCode1,
                    PrinterCode = _fixture.ApiSeedData.PrinterCode1
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateSigningDevices_ShouldBothFail_WhenPassingNonExistingLocationOrPrinterCodes()
    {
        var firstCreatedResponse = await new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.SigningDeviceEndpoints.CreateSigningDevice)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = Guid.NewGuid(),
                    Name = Guid.NewGuid(),
                    IsActive = true,
                    LocationCode = Guid.NewGuid(),
                    PrinterCode = _fixture.ApiSeedData.PrinterCode1
                })
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        firstCreatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        firstCreatedResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        firstCreatedResponse.responseContent!.Errors.ShouldContain(x => x.Slug == SigningDeviceErrors.ValidationLocationDoesNotExists.Code);
        firstCreatedResponse.responseContent!.Errors.ShouldContain(x => x.Message == SigningDeviceErrors.ValidationLocationDoesNotExists.Description);
        
        var secondCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.SigningDeviceEndpoints.CreateSigningDevice)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = Guid.NewGuid(),
                    Name = Guid.NewGuid(),
                    IsActive = true,
                    LocationCode = _fixture.ApiSeedData.LocationCode1,
                    PrinterCode = Guid.NewGuid()
                })
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        secondCreatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        secondCreatedResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        secondCreatedResponse.responseContent!.Errors.ShouldContain(x => x.Slug == SigningDeviceErrors.ValidationPrinterDoesNotExists.Code);
        secondCreatedResponse.responseContent!.Errors.ShouldContain(x => x.Message == SigningDeviceErrors.ValidationPrinterDoesNotExists.Description);
    }

    private async Task VerifySigningDeviceAsync(string code, string expectedName, bool expectedIsActive, string expectedLocationCode, string expectedPrinterCode)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.SigningDeviceEndpoints.GetSigningDevices}/{code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<SigningDeviceResponse>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Code.ShouldBe(code);
        getResponse.responseContent.Name.ShouldBe(expectedName);
        getResponse.responseContent.IsActive.ShouldBe(expectedIsActive);
        getResponse.responseContent.LocationCode.ShouldBe(expectedLocationCode);
        getResponse.responseContent.PrinterCode.ShouldBe(expectedPrinterCode);
    }
    
    private async Task VerifyNotFoundSigningDeviceAsync(string code)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.SigningDeviceEndpoints.GetSigningDevices}/{code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<SigningDeviceResponse>(ensureSuccessStatusCode: false);
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}