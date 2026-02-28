using System.Net;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Planning.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Lib.Shared.Errors.Models;

using Shouldly;

namespace Nexticz.Module.Mmo.Planning.Tests.Functional.ApiTests;

[Collection(nameof(PlanningApiCollection))]
public class CreateBatchApiTests(PlanningApiFactoryFixture fixture)
{
    private readonly HttpClient _client = fixture.Factory.CreateClient();
    
    [Fact]
    public async Task CreateBatch_ShouldReturnUnauthorized_WhenNotPassingAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, 
                    CreateBatchUrl("mycka_1"))
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task CreateBatch_ShouldSucceed_WhenPassingValidData()
    {
        const string washingMachineCode = "mycka_1";
        const string lineCode = $"{washingMachineCode}_L1";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchUrl(washingMachineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateCreateBatchRequest(
                        lineCode, "BOZ6000000633", "BO6099506280"))
                .SendAndDeserializeAsync<CreateBatchResponse>();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, PlanningEndpoints.WashingMachineEndpoints.GetWashingMachines)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .SendAndDeserializeAsync<WashingMachineResponse[]>();
        
        var washingMachineLineQueue = 
            getResponse.responseContent?
                .SelectMany(x => x.Queues)
                .FirstOrDefault(x => x.Code.Equals(lineCode, StringComparison.InvariantCultureIgnoreCase));
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        createdResponse.responseContent.ShouldNotBeNull();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        washingMachineLineQueue.ShouldNotBeNull();
        washingMachineLineQueue.Items.ShouldContain(x => x.Id == createdResponse.responseContent.Batch.Id);
    }
    
    [Fact]
    public async Task CreateSisterBatch_ShouldSucceed_WhenPassingValidData()
    {
        const string washingMachineCode = "mycka_7";
        const string lineCode = $"{washingMachineCode}_L1";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchUrl(washingMachineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateCreateBatchRequest(
                        lineCode, "BOZ6000003714", "BO6099516417", "BO6000802112"))
                .SendAndDeserializeAsync<CreateBatchResponse>();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, PlanningEndpoints.WashingMachineEndpoints.GetWashingMachines)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .SendAndDeserializeAsync<WashingMachineResponse[]>();

        var upperLineCode = lineCode.ToUpperInvariant();
        var washingMachine = 
            getResponse.responseContent?
                .FirstOrDefault(x => 
                    x.Queues.Any(y => y.Code == upperLineCode));

        var queue1 = washingMachine!.Queues.FirstOrDefault(x => x.Code == upperLineCode);
        var queue2 = washingMachine!.Queues.FirstOrDefault(x => x.Code != upperLineCode);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        createdResponse.responseContent.ShouldNotBeNull();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        queue1.ShouldNotBeNull();
        queue1.Items.ShouldContain(x => x.Id == createdResponse.responseContent.Batch.Id);
        queue2.ShouldNotBeNull();
        queue2.Items.ShouldContain(x => x.Id == createdResponse.responseContent.SisterBatch!.Id);
    }
    
    [Fact]
    public async Task CreateBatch_ShouldFail_WhenPassingSisterBatchToOneLineWashingMachine()
    {
        const string washingMachineCode = "mycka_3";
        const string lineCode = $"{washingMachineCode}_L1";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchUrl(washingMachineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateCreateBatchRequest(
                        lineCode, "BOZ6000003714", "BO6099516417", "BO6000802112"))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.Errors[0].Slug.ShouldBe(Application.WashingMachines.WashingMachineErrors.ValidationWashingMachineIsNotPresentedInFilteredWashingMachines.Code);
    }
    
    [Fact]
    public async Task CreateBatch_ShouldFail_WhenPassingPackagingDoesNotFitToWashingMachine()
    {
        const string washingMachineCode = "mycka_1";
        const string lineCode = $"{washingMachineCode}_L1";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchUrl(washingMachineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateCreateBatchRequest(
                        lineCode, "BOZ6000000515", "BO6099101208"))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.Errors[0].Slug.ShouldBe(Application.WashingMachines.WashingMachineErrors.ValidationWashingMachineIsNotPresentedInFilteredWashingMachines.Code);
    }
    
    [Fact]
    public async Task CreateBatch_ShouldFail_WhenKitDoesNotExist()
    {
        const string washingMachineCode = "mycka_1";
        const string lineCode = $"{washingMachineCode}_L1";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchUrl(washingMachineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateCreateBatchRequest(
                        lineCode, "NonExisting", "BO6099101208"))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.Errors[0].Slug.ShouldBe(Application.WashingMachines.WashingMachineErrors.ValidationKitWithCodeDoesNotExistInSettings.Code);
    }
    
    [Fact]
    public async Task CreateBatch_ShouldFail_WhenPackagingDoesNotExist()
    {
        const string washingMachineCode = "mycka_1";
        const string lineCode = $"{washingMachineCode}_L1";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchUrl(washingMachineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateCreateBatchRequest(
                        lineCode, "BOZ6000000515", "NonExisting"))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.Errors[0].Slug.ShouldBe(Application.WashingMachines.WashingMachineErrors.ValidationPackagingWithCodeDoesNotExistInSettings.Code);
    }
    
    [Fact]
    public async Task CreateBatch_ShouldFail_WhenPackagingIsNotInKit()
    {
        const string washingMachineCode = "mycka_7";
        const string lineCode = $"{washingMachineCode}_L1";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CreateBatchUrl(washingMachineCode))
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .WithContent(
                    GenerateCreateBatchRequest(
                        lineCode, "BOZ6000000515", "BO6000514362"))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.Errors[0].Slug.ShouldBe(Application.WashingMachines.WashingMachineErrors.ValidationPackagingIsNotListedInKitPackagings.Code);
    }
    

    public static CreateBatchRequest GenerateCreateBatchRequest(
        string lineQueueCode,
        string kitCode,
        string packagingCode,
        string sisterPackagingCode = "",
        int kitsCount = 10
        )
    {
        return new CreateBatchRequest(lineQueueCode, kitCode, kitsCount, packagingCode, sisterPackagingCode);
    }

    public static string CreateBatchUrl(string washingMachineCode)
        => PlanningEndpoints.WashingMachineEndpoints.CreateBatch.Replace("{washingMachineCode}", washingMachineCode);

}