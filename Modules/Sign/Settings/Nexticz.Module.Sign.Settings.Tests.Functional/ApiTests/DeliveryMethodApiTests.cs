using System.Net;
using Nexticz.Module.Sign.Settings.Contracts.DeliveryMethods;
using Nexticz.Module.Sign.Settings.Presentation;
using Nexticz.Module.Sign.SharedTesting;
using Shouldly;

namespace Nexticz.Module.Sign.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class DeliveryMethodApiTests
{
    private readonly SettingsApiFactoryFixture _fixture;
    private readonly HttpClient _client;

    public DeliveryMethodApiTests(SettingsApiFactoryFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Factory.CreateClient();
    }
    
    [Fact]
    public async Task HappyPathTestWithAllCrudEndpoints_ShouldCreateUpdateAndDeleteDeliveryMethod_WhenCallingAllEndpointsOneByOne()
    {
        const string code = "DeliveryMethodApiTests_DeliveryMethodCode";
        const string nameWhenCreated = "DeliveryMethodApiTests_DeliveryMethodName";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.DeliveryMethodEndpoints.CreateDeliveryMethod)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = code,
                    Name = nameWhenCreated,
                    LoadingDocumentPrintCopiesCount = 1,
                    DeliveryDocumentPrintCopiesCount = 1
                })
                .SendAndDeserializeAsync<DeliveryMethodResponse>();
        
        await VerifyDeliveryMethodAsync(createdResponse.responseContent!.Code, nameWhenCreated, 1, 1);
        
        var updateBody = new
        {
            Name = "DeliveryMethodApiTests_DeliveryMethodNameUpdated",
            LoadingDocumentPrintCopiesCount = 2,
            DeliveryDocumentPrintCopiesCount = 3
        };
        await new HttpRequestBuilder(_client, HttpMethod.Put,
                $"{SettingsEndpoints.DeliveryMethodEndpoints.GetDeliveryMethods}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateBody)
                .SendAsync();
        
        await VerifyDeliveryMethodAsync(createdResponse.responseContent!.Code, updateBody.Name, updateBody.LoadingDocumentPrintCopiesCount, updateBody.DeliveryDocumentPrintCopiesCount);;
        
        await new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.DeliveryMethodEndpoints.GetDeliveryMethods}/{createdResponse.responseContent.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();

        await VerifyNotFoundDeliveryMethodAsync(createdResponse.responseContent!.Code);
    }
    
    [Fact]
    public async Task CreateDeliveryMethod_ShouldFail_WhenPassingEmptyCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.DeliveryMethodEndpoints.CreateDeliveryMethod)
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
    public async Task CreateSameDeliveryMethods_ShouldSecondFail_WhenPassingSameCodes()
    {
        var sameCode = $"SameCode_{Guid.NewGuid()}";
        await new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.DeliveryMethodEndpoints.CreateDeliveryMethod)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = Guid.Empty,
                    LoadingDocumentPrintCopiesCount = 1,
                    DeliveryDocumentPrintCopiesCount = 1
                })
                .SendAsync();
        
        var secondCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.DeliveryMethodEndpoints.CreateDeliveryMethod)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = Guid.Empty,
                    LoadingDocumentPrintCopiesCount = 1,
                    DeliveryDocumentPrintCopiesCount = 1
                })
                .SendAsync();
        
        secondCreatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }

    private async Task VerifyDeliveryMethodAsync(string code, string expectedName, int loadingDocumentPrintCopiesCount, int deliveryDocumentPrintCopiesCount)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.DeliveryMethodEndpoints.GetDeliveryMethods}/{code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<DeliveryMethodResponse>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Code.ShouldBe(code);
        getResponse.responseContent.Name.ShouldBe(expectedName);
        getResponse.responseContent.LoadingDocumentPrintCopiesCount.ShouldBe(loadingDocumentPrintCopiesCount);
        getResponse.responseContent.DeliveryDocumentPrintCopiesCount.ShouldBe(deliveryDocumentPrintCopiesCount);
    }
    
    private async Task VerifyNotFoundDeliveryMethodAsync(string code)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.DeliveryMethodEndpoints.GetDeliveryMethods}/{code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<DeliveryMethodResponse>(ensureSuccessStatusCode: false);
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}