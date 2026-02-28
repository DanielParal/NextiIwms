using System.Net;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Planning.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Shouldly;

namespace Nexticz.Module.Mmo.Planning.Tests.Functional.ApiTests;

[Collection(nameof(PlanningApiCollection))]
public class ActivateBatchApiTests(PlanningApiFactoryFixture fixture)
{
    private readonly HttpClient _client = fixture.Factory.CreateClient();
    
    [Fact]
    public async Task ActivateBatch_ShouldSucceed_WhenPassingValidData()
    {
        const string washingMachineCode = "mycka_2";
        const string lineCode1 = $"{washingMachineCode}_L1";
        const string lineCode2 = $"{washingMachineCode}_L2";
        
        // Create batches in washing machine 2
        var createdResponse1 = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchApiTests.CreateBatchUrl(washingMachineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    CreateBatchApiTests.GenerateCreateBatchRequest(
                        lineCode1, "BOZ6000006372", "BO6000150439"))
                .SendAndDeserializeAsync<CreateBatchResponse>();
        var batch1 = createdResponse1.responseContent!.Batch;
        
        var createdResponse2Sister = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchApiTests.CreateBatchUrl(washingMachineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    CreateBatchApiTests.GenerateCreateBatchRequest(
                        lineCode1, "BOZ6000006372", "BO6000150439", "BO6000150439"))
                .SendAndDeserializeAsync<CreateBatchResponse>();
        var batch2Sister1 = createdResponse2Sister.responseContent!.Batch;
        var batch2Sister2 = createdResponse2Sister.responseContent!.SisterBatch;
        
        var createdResponse3 = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchApiTests.CreateBatchUrl(washingMachineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    CreateBatchApiTests.GenerateCreateBatchRequest(
                        lineCode2, "BOZ6000006372", "BO6000150439"))
                .SendAndDeserializeAsync<CreateBatchResponse>();
        var batch3 = createdResponse3.responseContent!.Batch;
        
        // try activate second sister response - should fail
        var activateResponse1 = await
            new HttpRequestBuilder(_client, HttpMethod.Post, ActivateBatchUrl(washingMachineCode, batch2Sister2!.Id))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateActivateBatchRequest(lineCode2))
                .SendAsync();
        
        activateResponse1.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        
        // activate batch 1 - should succeed
        var activateResponse2 = await
            new HttpRequestBuilder(_client, HttpMethod.Post, ActivateBatchUrl(washingMachineCode, batch1!.Id))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateActivateBatchRequest(lineCode1))
                .SendAsync();
        
        activateResponse2.StatusCode.ShouldBe(HttpStatusCode.OK);
        
        // activate batch 2 - should succeed and finish batch 1
        var activateResponse3 = await
            new HttpRequestBuilder(_client, HttpMethod.Post, ActivateBatchUrl(washingMachineCode, batch2Sister1!.Id))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateActivateBatchRequest(lineCode1))
                .SendAsync();
        
        activateResponse3.StatusCode.ShouldBe(HttpStatusCode.OK);
        
        // Assert
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, PlanningEndpoints.WashingMachineEndpoints.GetWashingMachines)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .SendAndDeserializeAsync<WashingMachineResponse[]>();
        
        var upperLineCode = lineCode1.ToUpperInvariant();
        var washingMachine = 
            getResponse.responseContent?
                .FirstOrDefault(x => 
                    x.Queues.Any(y => y.Code == upperLineCode));

        var queue1 = washingMachine!.Queues.FirstOrDefault(x => x.Code == upperLineCode);
        var queue2 = washingMachine!.Queues.FirstOrDefault(x => x.Code != upperLineCode);
        
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        queue1.ShouldNotBeNull();
        queue1.Items.ShouldNotContain(x => x.Id == batch1.Id);
        queue1.Items.ShouldContain(x => x.Id == batch2Sister1.Id);
        queue2.ShouldNotBeNull();
        queue2.Items.ShouldContain(x => x.Id == batch2Sister2.Id);
        
        var washingBatchSister1 = queue1.Items.First(x => x.Id == batch2Sister1.Id);
        washingBatchSister1.Status.ShouldBe(BatchStatusContract.Washing);
        
        var washingBatchSister2 = queue2.Items.First(x => x.Id == batch2Sister2.Id);
        washingBatchSister2.Status.ShouldBe(BatchStatusContract.Washing);
        
        var notWashingBatch = queue2.Items.First(x => x.Id == batch3.Id);
        notWashingBatch.Status.ShouldBe(BatchStatusContract.InQueue);
    }

    private static ActivateBatchRequest GenerateActivateBatchRequest(string lineQueueCode)
    {
        return new ActivateBatchRequest(lineQueueCode);
    }
    
    private static string ActivateBatchUrl(string washingMachineCode, Guid batchId)
        => PlanningEndpoints.WashingMachineEndpoints
            .ActivateBatch
            .Replace("{washingMachineCode}", washingMachineCode)
            .Replace("{batchId}", batchId.ToString());
}