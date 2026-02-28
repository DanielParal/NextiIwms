using System.Net;
using Nexticz.Module.Mmo.Drying.Presentation;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Planning.Presentation;
using Nexticz.Module.Mmo.Planning.Tests.Functional.ApiTests;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Module.Mmo.Washing.Presentation;
using Shouldly;

namespace Nexticz.Module.Mmo.FullWorkflowApiTests.ApiTests;

public class BaseApiTests(FullWorkflowTestsApiFactoryFixture fixture)
{
    protected readonly HttpClient Client = fixture.Factory.CreateClient();

    protected async Task ActivateBatchAsync(BatchContract batch, string washingMachineCode, string lineCode)
    {
        // activate batch 1 - should succeed
        var activateResponse = await
            new HttpRequestBuilder(Client, HttpMethod.Post, ActivateBatchUrl(washingMachineCode, batch.Id))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateActivateBatchRequest(lineCode))
                .SendAsync();
        
        activateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
    
    protected async Task<(BatchContract batch, BatchContract? sisterBatch)> CreateSisterBatchesAsync(
        string washingMachineCode, string lineCode, string kitCode, string packagingCode, string sisterPackagingCode)
    {
        // Create batches in washing machine 4
        var createdSisterResponse = await
            new HttpRequestBuilder(Client, HttpMethod.Post, CreateBatchApiTests.CreateBatchUrl(washingMachineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    CreateBatchApiTests.GenerateCreateBatchRequest(
                        lineCode, kitCode, packagingCode, sisterPackagingCode))
                .SendAndDeserializeAsync<CreateBatchResponse>();
        var batch1Sister1 = createdSisterResponse.responseContent!.Batch;
        var batch1Sister2 = createdSisterResponse.responseContent!.SisterBatch;
        return (batch1Sister1, batch1Sister2);
    }
    
    protected async Task WorkersLoginToTheLinesAsync(string lineCode1, string lineCode2)
    {
        await new HttpRequestBuilder(Client, HttpMethod.Post, EnterLineUrl(lineCode1))
            .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
            .WithContent(
                new
                {
                    workerPin = fixture.FullWorkflowApiSeedData.Worker1Pin
                })
            .SendAsync();
        
        await new HttpRequestBuilder(Client, HttpMethod.Post, EnterLineUrl(lineCode2))
            .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
            .WithContent(
                new
                {
                    workerPin = fixture.FullWorkflowApiSeedData.Worker2Pin
                })
            .SendAsync();
    }
    
    protected static ActivateBatchRequest GenerateActivateBatchRequest(string lineQueueCode) => new(lineQueueCode);
    
    protected static string EnterLineUrl(string lineCode)
        => WashingEndpoints.LastEnteredWorkerOnLineEndpoints
            .EnterLine
            .Replace("{lineCode}", lineCode);
    
    protected static string ActivateBatchUrl(string washingMachineCode, Guid batchId)
        => PlanningEndpoints.WashingMachineEndpoints
            .ActivateBatch
            .Replace("{washingMachineCode}", washingMachineCode)
            .Replace("{batchId}", batchId.ToString());
    
    protected static string FinishBatchUrl(Guid batchId)
        => WashingEndpoints.BatchesEndpoints
            .FinishKit
            .Replace("{batchId}", batchId.ToString());
    
    protected static string FinishDryingUrl(Guid kitId)
        => DryingEndpoints.KitEndpoints
            .FinishKit
            .Replace("{kitId}", kitId.ToString());
    
    protected static string TransferDryingUrl(Guid kitId)
        => DryingEndpoints.KitEndpoints
            .TransferKit
            .Replace("{kitId}", kitId.ToString());
    
    protected static string ConfirmBatchUrl(Guid batchId)
        => WashingEndpoints.BatchesEndpoints
            .ConfirmSpecialInformation
            .Replace("{batchId}", batchId.ToString());
}