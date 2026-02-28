using System.Net;
using Nexticz.Module.Mmo.Settings.Application.Workers;
using Nexticz.Module.Mmo.Settings.Contracts.Workers;
using Nexticz.Module.Mmo.Settings.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Errors.Models;

using Shouldly;

namespace Nexticz.Module.Mmo.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class WorkerApiTests
{
    private readonly HttpClient _client;

    public WorkerApiTests(SettingsApiFactoryFixture fixture)
    {
        _client = fixture.Factory.CreateClient();
    }
    
    [Fact]
    public async Task CreateWorker_ShouldReturnUnauthorized_WhenNotPassingAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WorkerEndpoints.CreateWorker)
                .WithContent(GenerateCreateRequest(pin: 1000))
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task CreateWorker_ShouldReturnForbidden_WhenNotPassingAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WorkerEndpoints.CreateWorker)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyMember)
                .WithContent(GenerateCreateRequest(pin: 1001))
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task CreateWorker_ShouldReturnCreatedWorker_WhenOneWorkerCreated()
    {
        var request = GenerateCreateRequest(pin: 1002);
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WorkerEndpoints.CreateWorker)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(request)
                .SendAndDeserializeAsync<WorkerResponse>();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.WorkerEndpoints.GetWorkers}/{createdResponse.responseContent!.Pin}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<WorkerResponse>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Id.ShouldBe(createdResponse.responseContent.Id);
        getResponse.responseContent.Name.ShouldBe(request.Name);
        getResponse.responseContent.Pin.ShouldBe(request.Pin);
        getResponse.responseContent.IsActive.ShouldBe(request.IsActive);
    }
    
    [Fact]
    public async Task CreateWorker_ShouldFail_WhenPassingEmptyName()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WorkerEndpoints.CreateWorker)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(pin: 1003, name: string.Empty))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        createdResponse.responseContent.Errors.First().Slug.ShouldBe(WorkerErrors.ValidationNameIsRequired.Code);
    }
    
    [Fact]
    public async Task CreateWorker_ShouldFail_WhenPassingSmallerPin()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WorkerEndpoints.CreateWorker)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(pin: 123))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        createdResponse.responseContent.Errors.First().Slug.ShouldBe(WorkerErrors.ValidationPinHasToHaveBetween4And8Digits.Code);
    }
    
    [Fact]
    public async Task CreateWorker_ShouldFail_WhenPassingLargerPin()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WorkerEndpoints.CreateWorker)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(pin: 123456789))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        createdResponse.responseContent.Errors.First().Slug.ShouldBe(WorkerErrors.ValidationPinHasToHaveBetween4And8Digits.Code);
    }
    
    [Fact]
    public async Task CreateWorker_ShouldSecondFail_WhenPassingSamePin()
    {
        const int samePin = 2000;
        var firstCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WorkerEndpoints.CreateWorker)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(pin: samePin))
                .SendAndDeserializeAsync<WorkerResponse>();
        
        var secondCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WorkerEndpoints.CreateWorker)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(pin: samePin))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        firstCreatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.Created);
        secondCreatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        secondCreatedResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        secondCreatedResponse.responseContent.Errors.First().Slug.ShouldBe(WorkerErrors.ValidationWorkerWithPinAlreadyExist.Code);
    }
    
    [Fact]
    public async Task GetWorkers_ShouldReturnSomething_WhenAtLeastOnePackagingCreated()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WorkerEndpoints.CreateWorker)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest(pin: 1004))
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.WorkerEndpoints.GetWorkers}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<WorkerResponse>>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.GetType().ShouldBe(typeof(FilteredResult<WorkerResponse>));
        getResponse.responseContent.Data.Count.ShouldBeGreaterThan(0);
    }
    
    [Fact]
    public async Task UpdateWorker_ShouldUpdateWorker_WhenPassingCorrectData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WorkerEndpoints.CreateWorker)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(pin: 1005))
                .SendAndDeserializeAsync<WorkerResponse>();
        
        var updatePackagingBody = GenerateUpdateRequest(pin: 1006, name: "UpdatedName", isActive: false);
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put, 
                    $"{SettingsEndpoints.WorkerEndpoints.GetWorkers}/{createdResponse.responseContent!.Id}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    updatePackagingBody)
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.WorkerEndpoints.GetWorkers}/{updatePackagingBody.Pin}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<WorkerResponse>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Id.ShouldBe(createdResponse.responseContent.Id);
        getResponse.responseContent.Name.ShouldBe(updatePackagingBody.Name);
        getResponse.responseContent.Pin.ShouldBe(updatePackagingBody.Pin);
        getResponse.responseContent.IsActive.ShouldBe(updatePackagingBody.IsActive);
    }
    
    [Fact]
    public async Task UpdateWorker_ShouldFail_WhenPassingAlreadyExistingPin()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WorkerEndpoints.CreateWorker)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(pin: 1007))
                .SendAndDeserializeAsync<WorkerResponse>();

        const int samePin = 1008;
        var createdResponse2 = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WorkerEndpoints.CreateWorker)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(pin: samePin))
                .SendAndDeserializeAsync<WorkerResponse>();
        
        var updatePackagingBody = GenerateUpdateRequest(pin: samePin);
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put, 
                    $"{SettingsEndpoints.WorkerEndpoints.GetWorkers}/{createdResponse.responseContent!.Id}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    updatePackagingBody)
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        updatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updatedResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        updatedResponse.responseContent.Errors.First().Slug.ShouldBe(WorkerErrors.ValidationWorkerWithPinAlreadyExist.Code);
    }
    
    [Fact]
    public async Task UpdateWorker_ShouldReturnNotFound_WhenKeyDoesNotExist()
    {
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put, 
                    $"{SettingsEndpoints.WorkerEndpoints.GetWorkers}/{Guid.NewGuid().ToString()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateUpdateRequest(pin: 1009))
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdateWorker_ShouldFail_WhenPassingEmptyName()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WorkerEndpoints.CreateWorker)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(pin: 1010))
                .SendAndDeserializeAsync<WorkerResponse>();
        
        var updatePackagingBody = GenerateUpdateRequest(pin: 1010, name: string.Empty);
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put, 
                    $"{SettingsEndpoints.WorkerEndpoints.GetWorkers}/{createdResponse.responseContent!.Id}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    updatePackagingBody)
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        updatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updatedResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        updatedResponse.responseContent.Errors.First().Slug.ShouldBe(WorkerErrors.ValidationNameIsRequired.Code);
    }

    [Fact]
    public async Task DeleteWorker_ShouldSucceed_WhenPassingValidData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WorkerEndpoints.CreateWorker)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest(pin: 1011))
                .SendAndDeserializeAsync<WorkerResponse>();
        
        var getResponseBeforeDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.WorkerEndpoints.GetWorkers}/{createdResponse.responseContent!.Pin}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<WorkerResponse>();
        
        var deleteResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete, 
                    $"{SettingsEndpoints.WorkerEndpoints.GetWorkers}/{createdResponse.responseContent!.Id}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        var getResponseAfterDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.WorkerEndpoints.GetWorkers}/{createdResponse.responseContent!.Pin}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<WorkerResponse>(ensureSuccessStatusCode: false);
        
        getResponseBeforeDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponseBeforeDelete.responseContent.ShouldNotBeNull();
        
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        
        getResponseAfterDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteWorker_ShouldReturnNotFound_WhenPassingNonExistingKey()
    {
        var deleteResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete, 
                    $"{SettingsEndpoints.WorkerEndpoints.GetWorkers}/{Guid.NewGuid()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        deleteResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        deleteResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        deleteResponse.responseContent!.Errors.ShouldContain(x => x.Slug == WorkerErrors.NotFoundWorkerWithId.Code);
    }
    
    
    private static UpdateWorkerRequest GenerateUpdateRequest(
        int pin,
        string? name = null,
        bool? isActive = null) => 
        new (
            Name: name ?? Guid.NewGuid().ToString(),
            Pin: pin,
            IsActive: isActive ?? true
        );
    
    private static CreateWorkerRequest GenerateCreateRequest(
        int pin,
        string? name = null,
        bool? isActive = null) => 
        new (
            Name: name ?? Guid.NewGuid().ToString(),
            Pin: pin,
            IsActive: isActive ?? true
        );
}