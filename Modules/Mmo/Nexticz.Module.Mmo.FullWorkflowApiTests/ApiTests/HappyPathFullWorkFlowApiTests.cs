using System.Net;
using Nexticz.Module.Mmo.Drying.Contracts.Kits;
using Nexticz.Module.Mmo.Drying.Presentation;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Planning.Presentation;
using Nexticz.Module.Mmo.Reporting.Contracts.DriedKits;
using Nexticz.Module.Mmo.Reporting.Contracts.Shifts;
using Nexticz.Module.Mmo.Reporting.Contracts.ShiftSettings;
using Nexticz.Module.Mmo.Reporting.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Module.Mmo.Washing.Presentation;
using Nexticz.Lib.Shared.DevExtreme;

using Shouldly;
using BatchContract = Nexticz.Module.Mmo.Planning.Contracts.WashingMachines.BatchContract;

namespace Nexticz.Module.Mmo.FullWorkflowApiTests.ApiTests;

[Collection(nameof(FullWorkflowTestsApiCollection))]
public class HappyPathFullWorkFlowApiTests(FullWorkflowTestsApiFactoryFixture fixture) : BaseApiTests(fixture)
{
    [Fact]
    public async Task FullWorkFlow_ShouldSucceed_WhenPassingValidData()
    {
        const string washingMachineCode = "mycka_4";
        const string lineCode1 = $"{washingMachineCode}_L1";
        const string lineCode2 = $"{washingMachineCode}_L2";

        // Create batches
        var sisterBatches1 = await CreateSisterBatchesAsync(washingMachineCode, lineCode1, "BOZ6000006372", "BO6000150439", "BO6000150439");
        var sisterBatches2 = await CreateSisterBatchesAsync(washingMachineCode, lineCode1, "BOZ6000006372", "BO6000150439", "BO6000150439");
        
        // activate batch 1
        await ActivateBatchAsync(sisterBatches1.batch, washingMachineCode, lineCode1);
        
        // enter workers to the lines
        await WorkersLoginToTheLinesAsync(lineCode1, lineCode2);
        
       // Finish 2 kits 
       var finishedKit1 = await FinishWashingKitAsync(sisterBatches1.batch);
       var finishedKit2 = await FinishWashingKitAsync(sisterBatches1.batch);
        
        // Assert
        await VerifyPlanningAsync(lineCode1, sisterBatches1.batch, sisterBatches1!.sisterBatch!);
        await VerifyWashingAsync(lineCode1, sisterBatches1.batch);
        await VerifyDryingInCompletingSectionAsync(finishedKit1, finishedKit2);
        await VerifyEmptyDriedReportingAsync();
        
        // activate batch 2
        await ActivateBatchAsync(sisterBatches2.batch, washingMachineCode, lineCode1);
        
        // Assert
        await VerifyPlanning2Async(lineCode1, sisterBatches1.batch, sisterBatches2.batch, sisterBatches1!.sisterBatch!, sisterBatches2!.sisterBatch!);
        await VerifyWashingAsync(lineCode1, sisterBatches2.batch);
        
        // transfer kit in drying
        await TransferDryingKitAsync(finishedKit1);
        await VerifyDryingAfterTransferringAsync(finishedKit1, finishedKit2);
        
        // finish drying
        await FinishDryingKitAsync(finishedKit1);
        await VerifyDryingAfterFinishAsync(finishedKit1, finishedKit2);
        await VerifyDriedReportingAsync(finishedKit1, finishedKit2);
    }

    private static bool IsCurrentShift(ShiftSettingContract shiftSetting)
    {
        var nowTime = TimeOnly.FromDateTime(DateTimeOffset.Now.DateTime);
        if (shiftSetting.StartTime <= shiftSetting.EndTime)
            return shiftSetting.StartTime <= nowTime && nowTime <= shiftSetting.EndTime;
        
        // when over midnight  18:00-6:00
        return  shiftSetting.StartTime <= nowTime || nowTime <= shiftSetting.EndTime;
    }
    
    private async Task VerifyDriedReportingAsync(KitWashCycleContract finishKit1, KitWashCycleContract finishKit2)
    {
        await WaitForAsyncProjectionToBeBuildAsync(2000, 3, async() =>
        {
            var getDriedReporting = await
                new HttpRequestBuilder(Client, HttpMethod.Get, ReportingEndpoints.DriedKitEndpoints.GetDriedKits)
                    .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                    .SendAndDeserializeAsync<FilteredResult<DriedKitResponse>>();

            getDriedReporting.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
            getDriedReporting.responseContent.ShouldNotBeNull();
            getDriedReporting.responseContent.Data.ShouldContain(x => x.Id == finishKit1.Id);
            getDriedReporting.responseContent.Data.ShouldNotContain(x => x.Id == finishKit2.Id);
        });
    }

    private static async Task WaitForAsyncProjectionToBeBuildAsync(int callTimeoutMs, int attempts, Func<Task> actionToBeExecuted)
    {
        while (attempts > 0)
        {
            try
            {
                await actionToBeExecuted();
                break;
            }
            catch (Exception)
            {
                
                attempts--;
                if (attempts > 0)
                {
                    await Task.Delay(callTimeoutMs);
                }
            }
        }
    }
    
    private async Task VerifyEmptyDriedReportingAsync()
    {
        var getDriedReporting = await
            new HttpRequestBuilder(Client, HttpMethod.Get, ReportingEndpoints.DriedKitEndpoints.GetDriedKits)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<DriedKitResponse>>();
        
        getDriedReporting.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getDriedReporting.responseContent.ShouldNotBeNull();
        getDriedReporting.responseContent.Data.ShouldBeEmpty();
    }

    private async Task TransferDryingKitAsync(KitWashCycleContract finishedKit1)
    {
        await new HttpRequestBuilder(Client, HttpMethod.Post, TransferDryingUrl(finishedKit1.Id))
            .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
            .SendAsync();
    }
    
    private async Task FinishDryingKitAsync(KitWashCycleContract finishedKit1)
    {
        await new HttpRequestBuilder(Client, HttpMethod.Post, FinishDryingUrl(finishedKit1.Id))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
    }

    private async Task VerifyDryingInCompletingSectionAsync(KitWashCycleContract finishKit1, KitWashCycleContract finishKit2)
    {
        var getDryingResponse = await
            new HttpRequestBuilder(Client, HttpMethod.Get, DryingEndpoints.KitEndpoints.GetKits)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<KitResponse>>();
        
        getDryingResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getDryingResponse.responseContent.ShouldNotBeNull();
        var kit1 = getDryingResponse.responseContent.Data.FirstOrDefault(x => x.Id == finishKit1.Id);
        kit1.ShouldNotBeNull();
        kit1.Destination.ShouldBe(KitDestinationContract.CompletingSection);
        
        var kit2 = getDryingResponse.responseContent.Data.FirstOrDefault(x => x.Id == finishKit2.Id);
        kit2.ShouldNotBeNull();
        kit2.Destination.ShouldBe(KitDestinationContract.CompletingSection);
    }
    
    private async Task VerifyDryingAfterFinishAsync(KitWashCycleContract finishKit1, KitWashCycleContract finishKit2)
    {
        var getDryingResponse = await
            new HttpRequestBuilder(Client, HttpMethod.Get, DryingEndpoints.KitEndpoints.GetKits)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<KitResponse>>();
        
        getDryingResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getDryingResponse.responseContent.ShouldNotBeNull();
        getDryingResponse.responseContent.Data.ShouldNotContain(x => x.Id == finishKit1.Id);
        getDryingResponse.responseContent.Data.ShouldContain(x => x.Id == finishKit2.Id);
    }
    
    private async Task VerifyDryingAfterTransferringAsync(KitWashCycleContract finishKit1, KitWashCycleContract finishKit2)
    {
        var getDryingResponse = await
            new HttpRequestBuilder(Client, HttpMethod.Get, DryingEndpoints.KitEndpoints.GetKits)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<KitResponse>>();
        
        getDryingResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getDryingResponse.responseContent.ShouldNotBeNull();
        
        var kit1 = getDryingResponse.responseContent.Data.FirstOrDefault(x => x.Id == finishKit1.Id);
        kit1.ShouldNotBeNull();
        kit1.Destination.ShouldBe(KitDestinationContract.DryingSection);
        
        var kit2 = getDryingResponse.responseContent.Data.FirstOrDefault(x => x.Id == finishKit2.Id);
        kit2.ShouldNotBeNull();
        kit2.Destination.ShouldBe(KitDestinationContract.CompletingSection);
    }

    private async Task VerifyWashingAsync(
        string lineCode, 
        BatchContract batch)
    {
        var getWashingResponse = await
            new HttpRequestBuilder(Client, HttpMethod.Get, $"{WashingEndpoints.BatchesEndpoints.GetBatchByLineCode}?linecode={lineCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<BatchResponse>();
        
        getWashingResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getWashingResponse.responseContent.ShouldNotBeNull();
        getWashingResponse.responseContent.Id.ShouldBe(batch.Id);
    }

    private async Task VerifyPlanningAsync(
        string lineCode, 
        BatchContract batch, 
        BatchContract sisterBatch)
    {
        var getPlanningResponse = await
            new HttpRequestBuilder(Client, HttpMethod.Get, PlanningEndpoints.WashingMachineEndpoints.GetWashingMachines)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<WashingMachineResponse[]>();
        
        var upperLineCode = lineCode.ToUpperInvariant();
        var washingMachine = 
            getPlanningResponse.responseContent?
                .FirstOrDefault(x => 
                    x.Queues.Any(y => y.Code == upperLineCode));

        var queue1 = washingMachine!.Queues.FirstOrDefault(x => x.Code == upperLineCode);
        var queue2 = washingMachine!.Queues.FirstOrDefault(x => x.Code != upperLineCode);
        
        
        getPlanningResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getPlanningResponse.responseContent.ShouldNotBeNull();
        queue1.ShouldNotBeNull();
        queue1.Items.ShouldContain(x => x.Id == batch.Id);
        queue2.ShouldNotBeNull();
        queue2.Items.ShouldContain(x => x.Id == sisterBatch!.Id);
    }
    
    private async Task VerifyPlanning2Async(
        string lineCode, 
        BatchContract batch, 
        BatchContract batch2, 
        BatchContract sisterBatch,
        BatchContract sisterBatch2)
    {
        var getPlanningResponse2 = await
            new HttpRequestBuilder(Client, HttpMethod.Get, PlanningEndpoints.WashingMachineEndpoints.GetWashingMachines)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<WashingMachineResponse[]>();
        
        var upperLineCode2 = lineCode.ToUpperInvariant();
        var washingMachine2 = 
            getPlanningResponse2.responseContent?
                .FirstOrDefault(x => 
                    x.Queues.Any(y => y.Code == upperLineCode2));

        var queue1 = washingMachine2!.Queues.FirstOrDefault(x => x.Code == upperLineCode2);
        var queue2 = washingMachine2!.Queues.FirstOrDefault(x => x.Code != upperLineCode2);
        
        
        getPlanningResponse2.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getPlanningResponse2.responseContent.ShouldNotBeNull();
        queue1.ShouldNotBeNull();
        queue1.Items.ShouldNotContain(x => x.Id == batch.Id);
        queue1.Items.ShouldContain(x => x.Id == batch2.Id);
        queue2.ShouldNotBeNull();
        queue2.Items.ShouldNotContain(x => x.Id == sisterBatch!.Id);
        queue2.Items.ShouldContain(x => x.Id == sisterBatch2!.Id);
    }

    private async Task<KitWashCycleContract> FinishWashingKitAsync(BatchContract batch)
    {
        var finishKit1 = await
            new HttpRequestBuilder(Client, HttpMethod.Post, FinishBatchUrl(batch.Id))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FinishKitResponse>();
        
        finishKit1.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        return finishKit1.responseContent!.KitWashCycle;
    }
}