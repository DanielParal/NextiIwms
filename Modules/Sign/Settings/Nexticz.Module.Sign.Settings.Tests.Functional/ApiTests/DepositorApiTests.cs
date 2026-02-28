using System.Net;
using Nexticz.Module.Sign.Settings.Application.Depositors;
using Nexticz.Module.Sign.Settings.Contracts.Depositors;
using Nexticz.Module.Sign.Settings.Presentation;
using Nexticz.Module.Sign.SharedTesting;
using Nexticz.Lib.Shared.Errors.Models;
using Shouldly;

namespace Nexticz.Module.Sign.Settings.Tests.Functional.ApiTests;

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
    public async Task HappyPathTestWithAllCrudEndpoints_ShouldCreateUpdateAndDeleteDepositor_WhenCallingAllEndpointsOneByOne()
    {
        const string code = "DepositorApiTests_DepositorCode";
        const string nameWhenCreated = "DepositorApiTests_DepositorName";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.DepositorEndpoints.CreateDepositor)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = code,
                    Name = nameWhenCreated,
                    DepositorGroupCode = _fixture.ApiSeedData.DepositorGroupCode,
                    DeliveryTemplateCode = _fixture.ApiSeedData.DeliveryDocumentTemplateCode,
                    LoadingTemplateCode = _fixture.ApiSeedData.LoadingDocumentTemplateCode,
                })
                .SendAndDeserializeAsync<DepositorResponse>();
        
        await VerifyDepositorAsync(
            createdResponse.responseContent!.Code, nameWhenCreated, _fixture.ApiSeedData.DepositorGroupCode, _fixture.ApiSeedData.DeliveryDocumentTemplateCode, _fixture.ApiSeedData.LoadingDocumentTemplateCode);
        
        var updateBody = new
        {
            Name = "DepositorApiTests_DepositorNameUpdated",
            DepositorGroupCode = _fixture.ApiSeedData.DepositorGroupCode,
            DeliveryTemplateCode = _fixture.ApiSeedData.DeliveryDocumentTemplateCode,
            LoadingTemplateCode = _fixture.ApiSeedData.LoadingDocumentTemplateCode,
        };
        await new HttpRequestBuilder(_client, HttpMethod.Put,
                $"{SettingsEndpoints.DepositorEndpoints.GetDepositors}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateBody)
                .SendAsync();
        
        await VerifyDepositorAsync(createdResponse.responseContent!.Code, updateBody.Name, updateBody.DepositorGroupCode, updateBody.DeliveryTemplateCode, updateBody.LoadingTemplateCode);
        
         await new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.DepositorEndpoints.GetDepositors}/{createdResponse.responseContent.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        await VerifyNotFoundDepositorAsync(createdResponse.responseContent!.Code);
    }
    
    [Fact]
    public async Task CreateDepositor_ShouldFail_WhenPassingEmptyCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.DepositorEndpoints.CreateDepositor)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = string.Empty,
                    Name = Guid.NewGuid(),
                    DepositorGroupCode = _fixture.ApiSeedData.DepositorGroupCode,
                    DeliveryTemplateCode = _fixture.ApiSeedData.DeliveryDocumentTemplateCode,
                    LoadingTemplateCode = _fixture.ApiSeedData.LoadingDocumentTemplateCode,
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateDepositor_ShouldFail_WhenPassingNonExistingDepositorGroupCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.DepositorEndpoints.CreateDepositor)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = Guid.NewGuid(),
                    Name = Guid.NewGuid(),
                    DepositorGroupCode = Guid.NewGuid(),
                    DeliveryTemplateCode = _fixture.ApiSeedData.DeliveryDocumentTemplateCode,
                    LoadingTemplateCode = _fixture.ApiSeedData.LoadingDocumentTemplateCode,
                })
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        createdResponse.responseContent!.Errors.ShouldContain(x => x.Slug == DepositorErrors.ValidationDepositorGroupDoesNotExist.Code);
        createdResponse.responseContent!.Errors.ShouldContain(x => x.Message == DepositorErrors.ValidationDepositorGroupDoesNotExist.Description);
    }

    private async Task VerifyDepositorAsync(string code, string expectedName, string expectedDepositorCode, string expectedDeliveryTemplateCode, string expectedLoadingTemplateCode)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.DepositorEndpoints.GetDepositors}/{code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<DepositorResponse>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Code.ShouldBe(code);
        getResponse.responseContent.Name.ShouldBe(expectedName);
        getResponse.responseContent.DepositorGroupCode.ShouldBe(expectedDepositorCode);
        getResponse.responseContent.DeliveryTemplateCode.ShouldBe(expectedDeliveryTemplateCode);
        getResponse.responseContent.LoadingTemplateCode.ShouldBe(expectedLoadingTemplateCode);
    }
    
    private async Task VerifyNotFoundDepositorAsync(string code)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.DepositorEndpoints.GetDepositors}/{code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<DepositorResponse>(ensureSuccessStatusCode: false);
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}