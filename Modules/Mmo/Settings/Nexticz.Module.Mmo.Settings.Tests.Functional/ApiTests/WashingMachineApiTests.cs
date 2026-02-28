using System.Net;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Settings.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Errors.Models;

using Shouldly;

namespace Nexticz.Module.Mmo.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class WashingMachineApiTests
{
    private readonly HttpClient _client;

    public WashingMachineApiTests(SettingsApiFactoryFixture fixture)
    {
        _client = fixture.Factory.CreateClient();
    }

    [Fact]
    public async Task CreateWashingMachine_ShouldReturnUnauthorized_WhenNotPassingAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithContent(GenerateCreateRequest())
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task CreateWashingMachine_ShouldReturnForbidden_WhenNotPassingAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyMember)
                .WithContent(GenerateCreateRequest())
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task CreateWashingMachine_ShouldReturnCreatedWashingMachine_WhenOneDepositorCreated()
    {
        var request = GenerateCreateRequest(numberOfLines: 2);
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(request)
                .SendAndDeserializeAsync<WashingMachineResponse>();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.WashingMachineEndpoints.GetWashingMachines}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<WashingMachineResponse>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Id.ShouldBe(createdResponse.responseContent.Id);
        getResponse.responseContent.Code.ShouldBe(request.Code);
        getResponse.responseContent.Length.ShouldBe(request.Length);
        getResponse.responseContent.MinWidth.ShouldBe(request.MinWidth);
        getResponse.responseContent.MaxWidth.ShouldBe(request.MaxWidth);
        getResponse.responseContent.MaxHeight.ShouldBe(request.MaxHeight);
        getResponse.responseContent.MaxWaterTemperature.ShouldBe(request.MaxWaterTemperature);
        getResponse.responseContent.MaxAirTemperature.ShouldBe(request.MaxAirTemperature);
        getResponse.responseContent.NumberOfLines.ShouldBe(request.NumberOfLines);
        getResponse.responseContent.Speed1.ShouldBe(request.Speed1);
        getResponse.responseContent.Speed2.ShouldBe(request.Speed2);
        getResponse.responseContent.Speed3.ShouldBe(request.Speed3);
        getResponse.responseContent.WashingMachineLines.Length.ShouldBe(request.NumberOfLines);
        getResponse.responseContent.WashingMachineLines[0].Code.ShouldBe(request.Code + "_L1");
        getResponse.responseContent.WashingMachineLines[0].IsActive.ShouldBe(true);
        getResponse.responseContent.WashingMachineLines[1].Code.ShouldBe(request.Code + "_L2");
        getResponse.responseContent.WashingMachineLines[1].IsActive.ShouldBe(true);
    }
    
    [Fact]
    public async Task CreateWashingMachine_ShouldReturnUnprocessableEntity_WhenPassingEmptyCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest(code: string.Empty))
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateWashingMachine_ShouldReturnUnprocessableEntity_WhenPassingLength0()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest(length: 0))
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateWashingMachine_ShouldReturnUnprocessableEntity_WhenPassingMaxHeight0()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest(maxHeight: 0))
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateWashingMachine_ShouldReturnUnprocessableEntity_WhenPassingSpeed1Zero()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest(speed1: 0))
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateWashingMachine_ShouldReturnUnprocessableEntity_WhenPassingSpeed2Zero()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest(speed2: 0))
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateWashingMachine_ShouldReturnUnprocessableEntity_WhenPassingSpeed3Zero()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest(speed3: 0))
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateWashingMachine_ShouldReturnUnprocessableEntity_WhenPassingMaxWidthGraterThanMinWidth()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest(maxWidth: 10, minWidth: 20))
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateWashingMachine_ShouldReturnUnprocessableEntity_WhenPassingNumberOfLinesGraterThan2()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest(numberOfLines: 3))
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateWashingMachine_ShouldSecondFail_WhenPassingSameCode()
    {
        const string sameCode = "WM1";
        var firstCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest(code: sameCode))
                .SendAsync();
        
        var secondCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest(code: sameCode))
                .SendAsync();
        
        secondCreatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task GetWashingMachines_ShouldReturnSomething_WhenAtLeastOneWashingMachineCreated()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<WashingMachineResponse>>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.GetType().ShouldBe(typeof(FilteredResult<WashingMachineResponse>));
        getResponse.responseContent.Data.Count.ShouldBeGreaterThan(0);
    }
    
    [Fact]
    public async Task UpdateWashingMachine_ShouldSucceed_WhenPassingValidData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<WashingMachineResponse>();
    
        var updateRequest = GenerateUpdateRequest(createdResponse.responseContent!.Code, createdResponse.responseContent!.NumberOfLines);
        await new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.WashingMachineEndpoints.GetWashingMachines}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateRequest)
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.WashingMachineEndpoints.GetWashingMachines}/{createdResponse.responseContent.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<WashingMachineResponse>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Code.ShouldBe(createdResponse.responseContent.Code);
        getResponse.responseContent.MaxAirTemperature.ShouldBe(updateRequest.MaxAirTemperature);
        getResponse.responseContent.MaxWaterTemperature.ShouldBe(updateRequest.MaxWaterTemperature);
        getResponse.responseContent.Speed1.ShouldBe(updateRequest.Speed1);
        getResponse.responseContent.Speed2.ShouldBe(updateRequest.Speed2);
        getResponse.responseContent.Speed3.ShouldBe(updateRequest.Speed3);
        getResponse.responseContent.MinWidth.ShouldBe(updateRequest.MinWidth);
    }
    
    [Fact]
    public async Task UpdateWashingMachine_ShouldReturnNotFound_WhenPassingNonExistingCode()
    {
        var updatedResponse = await 
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.WashingMachineEndpoints.GetWashingMachines}/{Guid.NewGuid().ToString()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateUpdateRequest("SomeCode", 1))
                .SendAsync();
    
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdateWashingMachine_ShouldReturnUnprocessableEntity_WhenPassingInvalidSpeed1()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<WashingMachineResponse>();
    
        var updateRequest = GenerateUpdateRequest(createdResponse.responseContent!.Code, createdResponse.responseContent!.NumberOfLines, speed1: 0);
        var updatedResponse =
            await new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.WashingMachineEndpoints.GetWashingMachines}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateRequest)
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task UpdateWashingMachine_ShouldReturnUnprocessableEntity_WhenPassingInvalidSpeed2()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<WashingMachineResponse>();
    
        var updateRequest = GenerateUpdateRequest(createdResponse.responseContent!.Code, createdResponse.responseContent!.NumberOfLines, speed2: 0);
        var updatedResponse =
            await new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.WashingMachineEndpoints.GetWashingMachines}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateRequest)
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task UpdateWashingMachine_ShouldReturnUnprocessableEntity_WhenPassingInvalidSpeed3()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<WashingMachineResponse>();
    
        var updateRequest = GenerateUpdateRequest(createdResponse.responseContent!.Code, createdResponse.responseContent!.NumberOfLines, speed3: 0);
        var updatedResponse =
            await new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.WashingMachineEndpoints.GetWashingMachines}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateRequest)
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task UpdateWashingMachine_ShouldReturnUnprocessableEntity_WhenPassingInvalidMaxAirTemperature()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<WashingMachineResponse>();
    
        var updateRequest = GenerateUpdateRequest(createdResponse.responseContent!.Code, createdResponse.responseContent!.NumberOfLines, maxAirTemperature: 0);
        var updatedResponse =
            await new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.WashingMachineEndpoints.GetWashingMachines}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateRequest)
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task UpdateWashingMachine_ShouldReturnUnprocessableEntity_WhenPassingInvalidMaxWaterTemperature()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<WashingMachineResponse>();
    
        var updateRequest = GenerateUpdateRequest(createdResponse.responseContent!.Code, createdResponse.responseContent!.NumberOfLines, maxWaterTemperature: 0);
        var updatedResponse =
            await new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.WashingMachineEndpoints.GetWashingMachines}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateRequest)
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task UpdateWashingMachine_ShouldReturnUnprocessableEntity_WhenPassingEmptyLines()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<WashingMachineResponse>();
    
        var updateRequest = GenerateUpdateRequest(createdResponse.responseContent!.Code, createdResponse.responseContent!.NumberOfLines, washingMachineLines: []);
        var updatedResponse =
            await new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.WashingMachineEndpoints.GetWashingMachines}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateRequest)
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        updatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updatedResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        updatedResponse.responseContent.Errors.Count.ShouldBeGreaterThan(0);
        updatedResponse.responseContent.Errors[0].Slug.ShouldBe(
            WashingMachineErrors.ValidationNumberOfLinesDoNotMatch(createdResponse.responseContent.NumberOfLines).Code);
        updatedResponse.responseContent.Errors[0].Message.ShouldBe(
            WashingMachineErrors.ValidationNumberOfLinesDoNotMatch(createdResponse.responseContent.NumberOfLines).Description);
    }
    
    [Fact]
    public async Task UpdateWashingMachine_ShouldReturnUnprocessableEntity_WhenPassingWrongNumberOfLines()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<WashingMachineResponse>();
    
        var updateRequest = GenerateUpdateRequest(createdResponse.responseContent!.Code, 1);
        var updatedResponse =
            await new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.WashingMachineEndpoints.GetWashingMachines}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateRequest)
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        updatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updatedResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        updatedResponse.responseContent.Errors.Count.ShouldBeGreaterThan(0);
        updatedResponse.responseContent.Errors[0].Slug.ShouldBe(
            WashingMachineErrors.ValidationNumberOfLinesDoNotMatch(createdResponse.responseContent.NumberOfLines).Code);
        updatedResponse.responseContent.Errors[0].Message.ShouldBe(
            WashingMachineErrors.ValidationNumberOfLinesDoNotMatch(createdResponse.responseContent.NumberOfLines).Description);
    }
    
    [Fact]
    public async Task UpdateWashingMachine_ShouldReturnUnprocessableEntity_WhenPassingInactiveLinesButActiveStatus()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<WashingMachineResponse>();
    
        var createdWashingMachineCode = createdResponse.responseContent!.Code;
        var updateRequest = GenerateUpdateRequest(
            createdResponse.responseContent!.Code, 
            createdResponse.responseContent!.NumberOfLines,
            washingMachineLines: [
                new WashingMachineLineContract($"{createdWashingMachineCode}_L1", false, new PrinterSettingsContract(null, null, null, null, null)), 
                new WashingMachineLineContract($"{createdWashingMachineCode}_L2", false, new PrinterSettingsContract(null, null, null, null, null))
            ],
            status: WashingMachineStatusContract.Working);
        var updatedResponse =
            await new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.WashingMachineEndpoints.GetWashingMachines}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateRequest)
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        updatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updatedResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        updatedResponse.responseContent.Errors.Count.ShouldBeGreaterThan(0);
        updatedResponse.responseContent.Errors[0].Slug.ShouldBe(
            WashingMachineErrors.ValidationAllLinesAreInactiveAndStatusWorking.Code);
        updatedResponse.responseContent.Errors[0].Message.ShouldBe(
            WashingMachineErrors.ValidationAllLinesAreInactiveAndStatusWorking.Description);
    }
    
    [Fact]
    public async Task UpdateWashingMachine_ShouldReturnUnprocessableEntity_WhenPassingWrongLineCodes()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<WashingMachineResponse>();
    
        var createdWashingMachineCode = createdResponse.responseContent!.Code;
        var updateRequest = GenerateUpdateRequest(
            createdResponse.responseContent!.Code, 
            createdResponse.responseContent!.NumberOfLines,
            washingMachineLines: [
                new WashingMachineLineContract($"WrongCode_L1", true, new PrinterSettingsContract(null, null, null, null, null)), 
                new WashingMachineLineContract($"{createdWashingMachineCode}_L2", true, new PrinterSettingsContract(null, null, null, null, null))
            ]);
        var updatedResponse =
            await new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.WashingMachineEndpoints.GetWashingMachines}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateRequest)
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        updatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updatedResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        updatedResponse.responseContent.Errors.Count.ShouldBeGreaterThan(0);
        updatedResponse.responseContent.Errors[0].Slug.ShouldBe(
            WashingMachineErrors.ValidationLineCodesMismatch(string.Empty, string.Empty).Code);
    }
    
    
    private static UpdateWashingMachineRequest GenerateUpdateRequest(
        string washingMachineCode,
        int numberOfLines,
        int? maxWaterTemperature = null,
        int? maxAirTemperature = null,
        int? speed1 = null,
        int? speed2 = null,
        int? speed3 = null,
        int? minWidth = null,
        WashingMachineStatusContract? status = null,
        WashingMachineLineContract[]? washingMachineLines = null) => 
        new (
            Note: "Updated note",
            Status: status ?? WashingMachineStatusContract.Working,
            MaxWaterTemperature: maxWaterTemperature ?? 180,
            MaxAirTemperature: maxAirTemperature ?? 140,
            Speed1: speed1 ?? 150,
            Speed2: speed2 ?? 165,
            Speed3: speed3 ?? 183,
            MinWidth: minWidth ?? 20,
            WashingMachineLines: washingMachineLines ?? CreateWashingMachineLines(numberOfLines, washingMachineCode)
        );
    
    private static WashingMachineLineContract[] CreateWashingMachineLines(int numberOfLines, string washingMachineCode)
    {
        var washingMachineLines = new WashingMachineLineContract[numberOfLines];
        for (var i = 0; i < numberOfLines; i++)
        {
            var code = $"{washingMachineCode}_L{1+i}";
            washingMachineLines[i] = new WashingMachineLineContract(code, true, new PrinterSettingsContract(null, null, null, null, null));
        }
        
        return washingMachineLines;
    }
    
    public static CreateWashingMachineRequest GenerateCreateRequest(
        string? code = null,
        int? length = null,
        int? minWidth = null,
        int? maxWidth = null,
        int? maxHeight = null,
        int? maxWaterTemperature = null,
        int? maxAirTemperature = null,
        int? numberOfLines = null,
        int? speed1 = null,
        int? speed2 = null,
        int? speed3 = null) => 
        new (
        Code: code ?? $"WM-{Guid.NewGuid().ToString().ToUpperInvariant()}",
        Note: "Created note",
        Status: WashingMachineStatusContract.Working,
        Length: length ?? 25000,
        MinWidth: minWidth ?? 30,
        MaxWidth: maxWidth ?? 250,
        MaxHeight: maxHeight ?? 420,
        MaxWaterTemperature: maxWaterTemperature ?? 80,
        MaxAirTemperature: maxAirTemperature ?? 40,
        NumberOfLines: numberOfLines ?? 2,
        Speed1: speed1 ?? 50,
        Speed2: speed2 ?? 65,
        Speed3: speed3 ?? 83
    );
}