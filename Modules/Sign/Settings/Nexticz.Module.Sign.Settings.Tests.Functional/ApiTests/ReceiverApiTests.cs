using System.Net;
using Nexticz.Module.Sign.Settings.Application.Receivers;
using Nexticz.Module.Sign.Settings.Contracts.Receivers;
using Nexticz.Module.Sign.Settings.Presentation;
using Nexticz.Module.Sign.SharedTesting;
using Nexticz.Lib.Shared.Errors.Models;
using Shouldly;

namespace Nexticz.Module.Sign.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class ReceiverApiTests
{
    private readonly SettingsApiFactoryFixture _fixture;
    private readonly HttpClient _client;

    public ReceiverApiTests(SettingsApiFactoryFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Factory.CreateClient();
    }
    
    [Fact]
    public async Task HappyPathTestWithAllCrudEndpoints_ShouldCreateUpdateAndDeleteReceiver_WhenCallingAllEndpointsOneByOne()
    {
        const string code = "ReceiverApiTests_ReceiverCode";
        const string nameWhenCreated = "ReceiverApiTests_ReceiverName";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ReceiverEndpoints.CreateReceiver)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = code,
                    PartnerCode = _fixture.ApiSeedData.PartnerCode,
                    Name = nameWhenCreated
                })
                .SendAndDeserializeAsync<ReceiverResponse>();
        
        await VerifyReceiverAsync(createdResponse.responseContent!.Code, createdResponse.responseContent!.PartnerCode, nameWhenCreated);
        
        var updateBody = new
        {
            Name = "ReceiverApiTests_ReceiverNameUpdated"
        };
        await new HttpRequestBuilder(_client, HttpMethod.Put,
                $"{SettingsEndpoints.ReceiverEndpoints.GetReceivers}/{createdResponse.responseContent!.Code}/{createdResponse.responseContent!.PartnerCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateBody)
                .SendAsync();
        
        await VerifyReceiverAsync(createdResponse.responseContent!.Code, createdResponse.responseContent!.PartnerCode, updateBody.Name);
        
        await new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.ReceiverEndpoints.GetReceivers}/{createdResponse.responseContent!.Code}/{createdResponse.responseContent!.PartnerCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();

        await VerifyNotFoundReceiverAsync(createdResponse.responseContent!.Code, createdResponse.responseContent!.PartnerCode);
    }
    
    [Fact]
    public async Task CreateReceiver_ShouldFail_WhenPassingEmptyCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ReceiverEndpoints.CreateReceiver)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = string.Empty,
                    PartnerCode = _fixture.ApiSeedData.PartnerCode,
                    Name = Guid.NewGuid()
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateReceiver_ShouldFail_WhenPassingNonExistingPartnerCode()
    {
        const string code = "ReceiverApiTests_ReceiverCode";
        const string nameWhenCreated = "ReceiverApiTests_ReceiverName";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ReceiverEndpoints.CreateReceiver)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = code,
                    PartnerCode = Guid.NewGuid(),
                    Name = nameWhenCreated
                })
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        createdResponse.responseContent!.Errors.ShouldContain(x => x.Slug == ReceiverErrors.ValidationPartnerDoesNotExist.Code);
        createdResponse.responseContent!.Errors.ShouldContain(x => x.Message == ReceiverErrors.ValidationPartnerDoesNotExist.Description);
    }

    private async Task VerifyReceiverAsync(string code, string partnerCode, string expectedName)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.ReceiverEndpoints.GetReceivers}/{code}/{partnerCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ReceiverResponse>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Code.ShouldBe(code);
        getResponse.responseContent.PartnerCode.ShouldBe(partnerCode);
        getResponse.responseContent.Name.ShouldBe(expectedName);
    }
    
    private async Task VerifyNotFoundReceiverAsync(string code, string partnerCode)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.ReceiverEndpoints.GetReceivers}/{code}/{partnerCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ReceiverResponse>(ensureSuccessStatusCode: false);
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}