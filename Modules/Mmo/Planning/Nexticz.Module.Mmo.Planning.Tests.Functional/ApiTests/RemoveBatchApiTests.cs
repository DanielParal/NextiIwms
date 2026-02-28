using System.Net;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Planning.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Lib.Shared.Errors.Models;

using Shouldly;

namespace Nexticz.Module.Mmo.Planning.Tests.Functional.ApiTests;

[Collection(nameof(PlanningApiCollection))]
public class RemoveBatchApiTests(PlanningApiFactoryFixture fixture)
{
    private readonly HttpClient _client = fixture.Factory.CreateClient();
    
    [Fact]
    public async Task RemoveBatch_ShouldReturnUnauthorized_WhenNotPassingAuthToken()
    {
        var deletedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete, 
                    RemoveBatchUrl("mycka_1", Guid.NewGuid().ToString()))
                .WithContent(GenerateRemoveBatchRequest("mycka_1_l1"))
                .SendAsync();
        
        deletedResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task RemoveBatch_ShouldSucceed_WhenPassingValidData()
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
        
        var deletedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete, 
                    RemoveBatchUrl(machineCode, createdResponse.responseContent!.Batch.Id.ToString()))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(GenerateRemoveBatchRequest(lineCode))
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, PlanningEndpoints.WashingMachineEndpoints.GetWashingMachines)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .SendAndDeserializeAsync<WashingMachineResponse[]>();
        
        var washingMachine = 
            getResponse.responseContent?
                .FirstOrDefault(x => 
                    x.Queues.Any(y => y.Code.Equals(lineCode, StringComparison.InvariantCultureIgnoreCase)));
        
        deletedResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        washingMachine!.Queues
            .SelectMany(x => x.Items)
            .ShouldNotContain(x => x.Id == createdResponse.responseContent!.Batch.Id);
    }
    
    [Fact]
    public async Task RemoveSisterBatch_ShouldSucceed_WhenPassingValidData()
    {
        const string machineCode = "mycka_7";
        const string lineCode = $"{machineCode}_l1";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchApiTests.CreateBatchUrl(machineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    CreateBatchApiTests.GenerateCreateBatchRequest(
                        lineCode, "BOZ6000070782", "BO6000660252", "BO6099504147"))
                .SendAndDeserializeAsync<CreateBatchResponse>();
        
        var deletedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete, 
                    RemoveBatchUrl(machineCode, createdResponse.responseContent!.Batch.Id.ToString()))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(GenerateRemoveBatchRequest(lineCode))
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, PlanningEndpoints.WashingMachineEndpoints.GetWashingMachines)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .SendAndDeserializeAsync<WashingMachineResponse[]>();
        
        var washingMachine = 
            getResponse.responseContent?
                .FirstOrDefault(x => 
                    x.Queues.Any(y => y.Code.Equals(lineCode, StringComparison.InvariantCultureIgnoreCase)));

        createdResponse.responseContent!.SisterBatch.ShouldNotBeNull();
        deletedResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        
        washingMachine!.Queues
            .SelectMany(x => x.Items)
            .ShouldNotContain(x => x.Id == createdResponse.responseContent!.Batch.Id);

        washingMachine!.Queues
            .SelectMany(x => x.Items)
            .ShouldNotContain(x => x.Id == createdResponse.responseContent!.SisterBatch.Id);
    }
    
    [Fact]
    public async Task RemoveBatch_ShouldFail_WhenPassingNonExistingLine()
    {
        const string machineCode = "mycka_1";
        const string lineCode = $"{machineCode}_l1";
        var deletedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete, 
                    RemoveBatchUrl(machineCode, Guid.NewGuid().ToString()))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(GenerateRemoveBatchRequest(lineCode))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        deletedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        deletedResponse.responseContent!.Errors[0].Slug.ShouldBe(WashingMachineErrors.ValidationBatchIsNotPresentInTheQueue.Code);
    }
    
    [Fact]
    public async Task RemoveBatch_ShouldFail_WhenPassingNonBatch()
    {
        const string machineCode = "mycka_7";
        const string lineCode = $"{machineCode}_l1";
        var deletedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete, 
                    RemoveBatchUrl(machineCode, Guid.NewGuid().ToString()))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(GenerateRemoveBatchRequest(lineCode))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        deletedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        deletedResponse.responseContent!.Errors[0].Slug.ShouldBe(WashingMachineErrors.ValidationBatchIsNotPresentInTheQueue.Code);
    }
    
    private static RemoveBatchRequest GenerateRemoveBatchRequest(string lineQueueCode)
    {
        return new RemoveBatchRequest(lineQueueCode);
    }

    private static string RemoveBatchUrl(string washingMachineCode, string batchId) 
        => PlanningEndpoints.WashingMachineEndpoints.RemoveBatch.Replace("{washingMachineCode}", washingMachineCode).Replace("{batchId}", batchId);
}