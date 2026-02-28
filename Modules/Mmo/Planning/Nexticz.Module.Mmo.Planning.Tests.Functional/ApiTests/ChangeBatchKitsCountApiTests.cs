using System.Net;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.Planning.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Lib.Shared.Errors.Models;

using Shouldly;

namespace Nexticz.Module.Mmo.Planning.Tests.Functional.ApiTests;

[Collection(nameof(PlanningApiCollection))]
public class UpdateBatchKisCountApiTests(PlanningApiFactoryFixture fixture)
{
    private readonly HttpClient _client = fixture.Factory.CreateClient();
    
    [Fact]
    public async Task UpdateBatchKisCount_ShouldReturnUnauthorized_WhenNotPassingAuthToken()
    {
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, 
                    ChangeBatchKitsCountUrl("mycka_1", Guid.NewGuid().ToString()))
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task UpdateBatchKisCount_ShouldSucceedAndIncreaseKitsCount_WhenPassingValidData()
    {
        const string machineCode = "mycka_7";
        const string lineCode = $"{machineCode}_l1";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchApiTests.CreateBatchUrl(machineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    CreateBatchApiTests.GenerateCreateBatchRequest(
                        lineCode, "BOZ6000070485", "BO6099504147", kitsCount: 5))
                .SendAndDeserializeAsync<CreateBatchResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, 
                    ChangeBatchKitsCountUrl(machineCode, createdResponse.responseContent!.Batch.Id.ToString()))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateUpdateBatchKitsCountRequest(
                        lineCode, 5))
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, PlanningEndpoints.WashingMachineEndpoints.GetWashingMachines)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .SendAndDeserializeAsync<WashingMachineResponse[]>();
        
        var washingMachine = 
            getResponse.responseContent?
                .FirstOrDefault(x => 
                    x.Queues.Any(y => y.Code.Equals(lineCode, StringComparison.InvariantCultureIgnoreCase)));
        var batch = washingMachine!.Queues.SelectMany(x => x.Items).First(x => x.Id == createdResponse.responseContent!.Batch.Id);
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        batch.KitsCount.ShouldBe(5);
    }
    
    [Fact]
    public async Task UpdateBatchKisCount_ShouldSucceedAndDecreaseKitsCount_WhenPassingValidData()
    {
        const string machineCode = "mycka_7";
        const string lineCode = $"{machineCode}_l1";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchApiTests.CreateBatchUrl(machineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    CreateBatchApiTests.GenerateCreateBatchRequest(
                        lineCode, "BOZ6000070485", "BO6099504147", kitsCount: 5))
                .SendAndDeserializeAsync<CreateBatchResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, 
                    ChangeBatchKitsCountUrl(machineCode, createdResponse.responseContent!.Batch.Id.ToString()))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateUpdateBatchKitsCountRequest(
                        lineCode, 4))
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, PlanningEndpoints.WashingMachineEndpoints.GetWashingMachines)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .SendAndDeserializeAsync<WashingMachineResponse[]>();
        
        var washingMachine = 
            getResponse.responseContent?
                .FirstOrDefault(x => 
                    x.Queues.Any(y => y.Code.Equals(lineCode, StringComparison.InvariantCultureIgnoreCase)));
        var batch = washingMachine!.Queues.SelectMany(x => x.Items).First(x => x.Id == createdResponse.responseContent!.Batch.Id);
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        batch.KitsCount.ShouldBe(4);
    }
    
    [Fact]
    public async Task UpdateSisterBatchKisCount_ShouldSucceed_WhenPassingValidData()
    {
        const string machineCode = "mycka_7";
        const string lineCode = $"{machineCode}_l1";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchApiTests.CreateBatchUrl(machineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    CreateBatchApiTests.GenerateCreateBatchRequest(
                        lineCode, "BOZ6000070782", "BO6000660252", "BO6099504147", kitsCount: 20))
                .SendAndDeserializeAsync<CreateBatchResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, 
                    ChangeBatchKitsCountUrl(machineCode, createdResponse.responseContent!.Batch.Id.ToString()))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateUpdateBatchKitsCountRequest(
                        lineCode, 24))
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, PlanningEndpoints.WashingMachineEndpoints.GetWashingMachines)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .SendAndDeserializeAsync<WashingMachineResponse[]>();
        
        var washingMachine = 
            getResponse.responseContent?
                .FirstOrDefault(x => 
                    x.Queues.Any(y => y.Code.Equals(lineCode, StringComparison.InvariantCultureIgnoreCase)));
        var batch = washingMachine!.Queues.SelectMany(x => x.Items).First(x => x.Id == createdResponse.responseContent!.Batch.Id);
        var sisterBatch = washingMachine!.Queues.SelectMany(x => x.Items).First(x => x.Id == createdResponse.responseContent!.SisterBatch!.Id);
        
        createdResponse.responseContent!.SisterBatch.ShouldNotBeNull();
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        
        batch.KitsCount.ShouldBe(24);
        sisterBatch.KitsCount.ShouldBe(24);
    }
    
    [Fact]
    public async Task UpdateBatchKisCount_ShouldFail_WhenPassingNonExistingLine()
    {
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, 
                    ChangeBatchKitsCountUrl("mycka_1", Guid.NewGuid().ToString()))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateUpdateBatchKitsCountRequest(
                        "NonExistingLineQueueCode", 4))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        updatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updatedResponse.responseContent!.Errors[0].Slug.ShouldBe(WashingMachineErrors.ValidationLineQueueDoesNotExist.Code);
    }
    
    [Fact]
    public async Task UpdateBatchKisCount_ShouldFail_WhenPassingNonExistingBatch()
    {
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, 
                    ChangeBatchKitsCountUrl("mycka_7", Guid.NewGuid().ToString()))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateUpdateBatchKitsCountRequest(
                        "mycka_7_l1", 4))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        updatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updatedResponse.responseContent!.Errors[0].Slug.ShouldBe(WashingMachineErrors.ValidationBatchIsNotPresentInTheQueue.Code);
    }
    
    [Fact]
    public async Task UpdateBatchKisCount_ShouldFail_WhenKitsCountToChangeForDecreaseIsGreaterThanKitsCount()
    {
        const string machineCode = "mycka_7";
        const string lineCode = $"{machineCode}_l1";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchApiTests.CreateBatchUrl(machineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    CreateBatchApiTests.GenerateCreateBatchRequest(
                        lineCode, "BOZ6000070485", "BO6099504147", kitsCount: 5))
                .SendAndDeserializeAsync<CreateBatchResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, 
                ChangeBatchKitsCountUrl(machineCode, createdResponse.responseContent!.Batch.Id.ToString()))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateUpdateBatchKitsCountRequest(
                        lineCode, -6))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        updatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updatedResponse.responseContent!.Errors[0].Slug.ShouldBe(BatchErrors.ValidationNotEnoughKitsLeftForDecrease(5).Code);
    }

    private static ChangeBatchKitsCountRequest GenerateUpdateBatchKitsCountRequest(string lineQueueCode, int kitsCountToChange)
    {
        return new ChangeBatchKitsCountRequest(lineQueueCode, kitsCountToChange);
    }
    
    private static string ChangeBatchKitsCountUrl(string washingMachineCode, string batchId) 
        => PlanningEndpoints.WashingMachineEndpoints.ChangeBatchKitsCount.Replace("{washingMachineCode}", washingMachineCode).Replace("{batchId}", batchId);
}