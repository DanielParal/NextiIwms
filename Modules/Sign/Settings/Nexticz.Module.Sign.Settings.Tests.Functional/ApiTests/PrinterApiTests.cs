using System.Net;
using Nexticz.Module.Sign.Settings.Application.Printers;
using Nexticz.Module.Sign.Settings.Contracts.Printers;
using Nexticz.Module.Sign.Settings.Presentation;
using Nexticz.Module.Sign.SharedTesting;
using Nexticz.Lib.Shared.Errors.Models;
using Shouldly;

namespace Nexticz.Module.Sign.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class PrinterApiTests
{
    private readonly SettingsApiFactoryFixture _fixture;
    private readonly HttpClient _client;

    public PrinterApiTests(SettingsApiFactoryFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Factory.CreateClient();
    }
    
    [Fact]
    public async Task HappyPathTestWithAllCrudEndpoints_ShouldCreateUpdateAndDeletePrinter_WhenCallingAllEndpointsOneByOne()
    {
        const string code = "PrinterApiTests_PrinterCode";
        const string nameWhenCreated = "PrinterApiTests_PrinterName";
        const string ipWhenCreated = "192.160.100.2";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PrinterEndpoints.CreatePrinter)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = code,
                    Name = nameWhenCreated,
                    Ip = ipWhenCreated,
                })
                .SendAndDeserializeAsync<PrinterResponse>();
        
        await VerifyPrinterAsync(createdResponse.responseContent!.Code, nameWhenCreated, ipWhenCreated);
        
        var updateBody = new
        {
            Name = "PrinterApiTests_PrinterNameUpdated",
            Ip = "192.160.100.3"       
        };
        await new HttpRequestBuilder(_client, HttpMethod.Put,
                $"{SettingsEndpoints.PrinterEndpoints.GetPrinters}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateBody)
                .SendAsync();
        
        await VerifyPrinterAsync(createdResponse.responseContent!.Code, updateBody.Name, updateBody.Ip);
        
        await new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.PrinterEndpoints.GetPrinters}/{createdResponse.responseContent.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();

        await VerifyNotFoundPrinterAsync(createdResponse.responseContent!.Code);
    }
    
    [Fact]
    public async Task CreatePrinter_ShouldFail_WhenPassingEmptyCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PrinterEndpoints.CreatePrinter)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = string.Empty,
                    Name = Guid.NewGuid(),
                    Ip = Guid.NewGuid()
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreatePrinter_ShouldFail_WhenPassingEmptyIp()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PrinterEndpoints.CreatePrinter)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = Guid.NewGuid(),
                    Name = Guid.NewGuid(),
                    Ip = string.Empty
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateSamePrinters_ShouldSecondFail_WhenPassingSameCodes()
    {
        var sameCode = $"SameCode_{Guid.NewGuid()}";
        await new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PrinterEndpoints.CreatePrinter)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = Guid.Empty
                })
                .SendAsync();
        
        var secondCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PrinterEndpoints.CreatePrinter)
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
    public async Task DeletePrinter_ShouldFail_WhenCodeStillUsedInSigningDevice()
    {
        var deletedResponse = await new HttpRequestBuilder(_client, HttpMethod.Delete,
                $"{SettingsEndpoints.PrinterEndpoints.GetPrinters}/{_fixture.ApiSeedData.PrinterCode1}")
            .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
            .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        deletedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        deletedResponse.responseContent!.Errors.First().Slug.ShouldBe(PrinterErrors.ValidationCodeIsUsedInSigningDevices(_fixture.ApiSeedData.SigningDeviceCode).Code);
        deletedResponse.responseContent!.Errors.First().Message.ShouldBe(PrinterErrors.ValidationCodeIsUsedInSigningDevices(_fixture.ApiSeedData.SigningDeviceCode).Description);
    }

    private async Task VerifyPrinterAsync(string code, string expectedName, string expectedIp)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.PrinterEndpoints.GetPrinters}/{code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<PrinterResponse>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Code.ShouldBe(code);
        getResponse.responseContent.Name.ShouldBe(expectedName);
        getResponse.responseContent.Ip.ShouldBe(expectedIp);
    }
    
    private async Task VerifyNotFoundPrinterAsync(string code)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.PrinterEndpoints.GetPrinters}/{code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<PrinterResponse>(ensureSuccessStatusCode: false);
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}