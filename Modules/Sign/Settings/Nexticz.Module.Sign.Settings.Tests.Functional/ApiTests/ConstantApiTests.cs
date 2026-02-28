using System.Net;
using Nexticz.Module.Sign.Settings.Application.Constants;
using Nexticz.Module.Sign.Settings.Contracts.Constants;
using Nexticz.Module.Sign.Settings.Domain.ConstantAggregate;
using Nexticz.Module.Sign.Settings.Presentation;
using Nexticz.Module.Sign.SharedTesting;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Errors.Models;
using Shouldly;

namespace Nexticz.Module.Sign.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class ConstantApiTests
{
    private readonly HttpClient _client;

    public ConstantApiTests(SettingsApiFactoryFixture fixture)
    {
        _client = fixture.Factory.CreateClient();
    }
    
    [Fact]
    public async Task CreateConstant_ShouldReturnUnauthorized_WhenNotPassingAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ConstantEndpoints.CreateConstant)
                .WithContent(GenerateCreateRequest())
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task CreateConstant_ShouldReturnCreatedConstant_WhenOneConstantCreated()
    {
        var request = GenerateCreateRequest();
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ConstantEndpoints.CreateConstant)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(request)
                .SendAndDeserializeAsync<ConstantResponse>();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.ConstantEndpoints.GetConstants}/{createdResponse.responseContent!.Key}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ConstantResponse>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Id.ShouldBe(createdResponse.responseContent.Id);
        getResponse.responseContent.Key.ShouldBe(request.Key);
        getResponse.responseContent.Value.ShouldBe(request.Value);
        getResponse.responseContent.ConstantType.ShouldBe(request.ConstantType);
        getResponse.responseContent.Description.ShouldBe(request.Description);
    }
    
    [Fact]
    public async Task CreateConstant_ShouldFail_WhenPassingNullKey()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ConstantEndpoints.CreateConstant)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(key: string.Empty))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        createdResponse.responseContent.Errors.First().Slug.ShouldBe(ConstantErrors.ValidationKeyIsRequired.Code);
    }
    
    [Fact]
    public async Task CreateConstant_ShouldFail_WhenPassingNullValue()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ConstantEndpoints.CreateConstant)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(value: string.Empty))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        createdResponse.responseContent.Errors.First().Slug.ShouldBe(ConstantErrors.ValidationValueIsNotCorrectType(string.Empty, ConstantType.Int32).Code);
    }
    
    [Fact]
    public async Task CreateConstant_ShouldFail_WhenPassingIncorrectCombinationOfValueAndType()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ConstantEndpoints.CreateConstant)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(value: "string", constantTypeContract: ConstantTypeContract.Int32))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        createdResponse.responseContent.Errors.First().Slug.ShouldBe(ConstantErrors.ValidationValueIsNotCorrectType("string", ConstantType.Int32).Code);
    }
    
    [Fact]
    public async Task GetConstants_ShouldReturnSomething_WhenAtLeastOnePackagingCreated()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ConstantEndpoints.CreateConstant)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.ConstantEndpoints.GetConstants}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<ConstantResponse>>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.GetType().ShouldBe(typeof(FilteredResult<ConstantResponse>));
        getResponse.responseContent.Data.Count.ShouldBeGreaterThan(0);
    }
    
    [Fact]
    public async Task UpdateConstant_ShouldUpdateConstant_WhenPassingCorrectData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ConstantEndpoints.CreateConstant)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest())
                .SendAndDeserializeAsync<ConstantResponse>();
        
        var updatePackagingBody = GenerateUpdateRequest();
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put, 
                    $"{SettingsEndpoints.ConstantEndpoints.GetConstants}/{createdResponse.responseContent!.Key}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    updatePackagingBody)
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.ConstantEndpoints.GetConstants}/{createdResponse.responseContent!.Key}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ConstantResponse>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Id.ShouldBe(createdResponse.responseContent.Id);
        getResponse.responseContent.Value.ShouldBe(updatePackagingBody.Value);
        getResponse.responseContent.Description.ShouldBe(updatePackagingBody.Description);
    }
    
    [Fact]
    public async Task UpdateConstant_ShouldUFail_WhenPassingIncorrectCombinationOfValueAndType()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ConstantEndpoints.CreateConstant)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest())
                .SendAndDeserializeAsync<ConstantResponse>();
        
        var updatePackagingBody = GenerateUpdateRequest(value: "SomeStringInsteadOfInt32");
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put, 
                    $"{SettingsEndpoints.ConstantEndpoints.GetConstants}/{createdResponse.responseContent!.Key}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    updatePackagingBody)
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        updatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updatedResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        updatedResponse.responseContent.Errors.First().Slug.ShouldBe(ConstantErrors.ValidationValueIsNotCorrectType("SomeStringInsteadOfInt32", ConstantType.Int32).Code);
    }
    
    [Fact]
    public async Task UpdateConstant_ShouldUFail_WhenPassingNullValue()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ConstantEndpoints.CreateConstant)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest())
                .SendAndDeserializeAsync<ConstantResponse>();
        
        var updatePackagingBody = GenerateUpdateRequest(value: string.Empty);
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put, 
                    $"{SettingsEndpoints.ConstantEndpoints.GetConstants}/{createdResponse.responseContent!.Key}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    updatePackagingBody)
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        updatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updatedResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        updatedResponse.responseContent.Errors.First().Slug.ShouldBe(ConstantErrors.ValidationValueIsNotCorrectType(string.Empty, ConstantType.Int32).Code);
    }
    
    [Fact]
    public async Task UpdateConstant_ShouldReturnNotFound_WhenKeyDoesNotExist()
    {
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put, 
                    $"{SettingsEndpoints.ConstantEndpoints.GetConstants}/{Guid.NewGuid().ToString()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateUpdateRequest())
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteConstant_ShouldSucceed_WhenPassingValidData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ConstantEndpoints.CreateConstant)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<ConstantResponse>();
        
        var getResponseBeforeDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.ConstantEndpoints.GetConstants}/{createdResponse.responseContent!.Key}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ConstantResponse>();
        
        var deleteResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete, 
                    $"{SettingsEndpoints.ConstantEndpoints.GetConstants}/{createdResponse.responseContent!.Key}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        var getResponseAfterDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.ConstantEndpoints.GetConstants}/{createdResponse.responseContent!.Key}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ConstantResponse>(ensureSuccessStatusCode: false);
        
        getResponseBeforeDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponseBeforeDelete.responseContent.ShouldNotBeNull();
        
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        
        getResponseAfterDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteConstant_ShouldReturnNotFound_WhenPassingNonExistingKey()
    {
        var deleteResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete, 
                    $"{SettingsEndpoints.ConstantEndpoints.GetConstants}/{Guid.NewGuid()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        deleteResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        deleteResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        deleteResponse.responseContent!.Errors.ShouldContain(x => x.Slug == ConstantErrors.KeyDoesNotExist.Code);
    }
    
    
    private static UpdateConstantRequest GenerateUpdateRequest(
        string? value = null,
        string? description = null) => 
        new (
            Value: value ?? "200",
            Description: description ?? "Some description updated"
        );
    
    private static CreateConstantRequest GenerateCreateRequest(
        string? key = null,
        string? value = null,
        ConstantTypeContract? constantTypeContract = null,
        string? description = null) => 
        new (
            Key: key ?? Guid.NewGuid().ToString(),
            Value: value ?? "100",
            ConstantType: constantTypeContract ?? ConstantTypeContract.Int32,
            Description: description ?? "Some description"
        );
}