using System.Net;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.Planning.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Lib.Shared.Errors.Models;

using Shouldly;

namespace Nexticz.Module.Mmo.Planning.Tests.Functional.ApiTests;

[Collection(nameof(PlanningApiCollection))]
public class MoveBatchInQueueApiTests(PlanningApiFactoryFixture fixture)
{
    private readonly HttpClient _client = fixture.Factory.CreateClient();
    
    [Fact]
    public async Task MoveBatchInQueue_ShouldReturnUnauthorized_WhenNotPassingAuthToken()
    {
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, 
                MoveBatchUrl("mycka_1", Guid.NewGuid().ToString()))
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task MoveBatchInQueue_ShouldSucceedAndMovedToFirstPosition_WhenPassingValidData()
    {
        const string machineCode = "mycka_7";
        const string lineCode = $"{machineCode}_l1";
        var createdResponse1 = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchApiTests.CreateBatchUrl(machineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    CreateBatchApiTests.GenerateCreateBatchRequest(
                        lineCode, "BOZ6000070485", "BO6099504147"))
                .SendAndDeserializeAsync<CreateBatchResponse>();
        
        var createdResponse2 = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchApiTests.CreateBatchUrl(machineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    CreateBatchApiTests.GenerateCreateBatchRequest(
                        lineCode, "BOZ6000070485", "BO6099504147"))
                .SendAndDeserializeAsync<CreateBatchResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, 
                MoveBatchUrl(machineCode, createdResponse2.responseContent!.Batch.Id.ToString()))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateMoveBatchInQueueRequest(
                        lineCode, InQueueMovementContract.FirstInQueue))
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, PlanningEndpoints.WashingMachineEndpoints.GetWashingMachines)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .SendAndDeserializeAsync<WashingMachineResponse[]>();
        
        var washingMachine = 
            getResponse.responseContent?
                .FirstOrDefault(x => 
                    x.Queues.Any(y => y.Code.Equals(lineCode, StringComparison.InvariantCultureIgnoreCase)));
        var firstBatch = washingMachine!.Queues.SelectMany(x => x.Items).First();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        firstBatch.Id.ShouldBe(createdResponse2.responseContent!.Batch.Id);
    }
    
    
    [Fact]
    public async Task MoveBatchInQueue_ShouldFail_WhenPassingNonExistingBatch()
    {
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, 
                    MoveBatchUrl("mycka_7", Guid.NewGuid().ToString()))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateMoveBatchInQueueRequest(
                        "mycka_7_l1", InQueueMovementContract.OneDown))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        updatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updatedResponse.responseContent!.Errors[0].Slug.ShouldBe(WashingMachineErrors.ValidationBatchIsNotPresentInTheQueue.Code);
    }
    
    [Fact]
    public async Task MoveBatchInQueue_ShouldFail_WhenPassingDifferentLineCode()
    {
        const string machineCode = "mycka_7";
        const string lineCode = $"{machineCode}_l1";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchApiTests.CreateBatchUrl(machineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    CreateBatchApiTests.GenerateCreateBatchRequest(
                        lineCode, "BOZ6000070485", "BO6099504147"))
                .SendAndDeserializeAsync<CreateBatchResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, 
                    MoveBatchUrl(machineCode, createdResponse.responseContent!.Batch.Id.ToString()))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateMoveBatchInQueueRequest(
                        $"{machineCode}_l2", InQueueMovementContract.OneDown))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        updatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updatedResponse.responseContent!.Errors[0].Slug.ShouldBe(WashingMachineErrors.ValidationBatchIsNotPresentInTheQueue.Code);
    }
    
    private static MoveBatchInQueueRequest GenerateMoveBatchInQueueRequest(string lineQueueCode, InQueueMovementContract movement)
    {
        return new MoveBatchInQueueRequest(lineQueueCode, movement);
    }
    
    private static string MoveBatchUrl(string washingMachineCode, string batchId)
        => PlanningEndpoints.WashingMachineEndpoints.MoveBatchInQueue.Replace("{washingMachineCode}", washingMachineCode).Replace("{batchId}", batchId);
    
}