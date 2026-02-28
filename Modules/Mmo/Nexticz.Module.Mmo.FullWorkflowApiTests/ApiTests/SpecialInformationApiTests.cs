using System.Net;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Module.Mmo.Washing.Application.Batches;
using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Lib.Shared.Errors.Models;

using Shouldly;
using BatchContract = Nexticz.Module.Mmo.Planning.Contracts.WashingMachines.BatchContract;

namespace Nexticz.Module.Mmo.FullWorkflowApiTests.ApiTests;

[Collection(nameof(FullWorkflowTestsApiCollection))]
public class SpecialInformationApiTests(FullWorkflowTestsApiFactoryFixture fixture) : BaseApiTests(fixture)
{
    private readonly FullWorkflowTestsApiFactoryFixture _fixture = fixture;

    [Fact]
    public async Task SpecialInformation_ShouldFirstFailAndAfterSucceed_WhenFirstOneIsConfirmedAndAfterSecondIsConfirmed()
    {
        const string washingMachineCode = "mycka_7";
        const string lineCode1 = $"{washingMachineCode}_L1";
        const string lineCode2 = $"{washingMachineCode}_L2";
        
        var sisterBatches1 = await CreateSisterBatchesAsync(washingMachineCode, lineCode1, 
            _fixture.FullWorkflowApiSeedData.KitCodeWithSpecialInformation, "BO6000152367", "BO6000101207");
        
        await ActivateBatchAsync(sisterBatches1.batch, washingMachineCode, lineCode1);
        
        await ValidateFailedNotLoginFinishKitAsync(sisterBatches1.batch);
        
        await WorkersLoginToTheLinesAsync(lineCode1, lineCode2);

        await ConfirmSpecialInformationAsync(sisterBatches1.batch);
        
        await ValidateFailedNotConfirmedFinishKitAsync(sisterBatches1.batch);
        
        await ConfirmSpecialInformationAsync(sisterBatches1.sisterBatch!);
        
        await ValidateSuccessFinishKitAsync(sisterBatches1.batch);
        await ValidateSuccessFinishKitAsync(sisterBatches1.sisterBatch!);
    }
    
    private async Task ValidateSuccessFinishKitAsync(BatchContract batch)
    {
        var finishKitResponse = await
            new HttpRequestBuilder(Client, HttpMethod.Post, FinishBatchUrl(batch.Id))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FinishKitResponse>();
        
        finishKitResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
    
    private async Task ValidateFailedNotLoginFinishKitAsync(BatchContract batch)
    {
        var finishKitResponse = await
            new HttpRequestBuilder(Client, HttpMethod.Post, FinishBatchUrl(batch.Id))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        finishKitResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        finishKitResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        finishKitResponse.responseContent!.Errors.ShouldContain(x => x.Slug == BatchErrors.ValidationThereIsNoWorkerAtTheLine.Code);
        finishKitResponse.responseContent!.Errors.ShouldContain(x => x.Message == BatchErrors.ValidationThereIsNoWorkerAtTheLine.Description);
    }
    
    private async Task ValidateFailedNotConfirmedFinishKitAsync(BatchContract batch)
    {
        var finishKitResponse = await
            new HttpRequestBuilder(Client, HttpMethod.Post, FinishBatchUrl(batch.Id))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        finishKitResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        finishKitResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        finishKitResponse.responseContent!.Errors.ShouldContain(x => x.Slug == BatchErrors.ValidationBatchIsNotConfirmedByWorker.Code);
        finishKitResponse.responseContent!.Errors.ShouldContain(x => x.Message == BatchErrors.ValidationBatchIsNotConfirmedByWorker.Description);
    }

    private async Task ConfirmSpecialInformationAsync(BatchContract batch)
    {
        var confirmResponse = await
            new HttpRequestBuilder(Client, HttpMethod.Post, ConfirmBatchUrl(batch.Id))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        confirmResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}