using System.Net;
using Nexticz.Module.Mmo.Settings.Application.Packagings;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Module.Mmo.Settings.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Errors.Models;

using Shouldly;

namespace Nexticz.Module.Mmo.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class PackagingApiTests
{
    private readonly SettingsApiFactoryFixture _fixture;
    private readonly HttpClient _client;
    
    public PackagingApiTests(SettingsApiFactoryFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Factory.CreateClient();
    }
    
    [Fact]
    public async Task CreatePackaging_ShouldReturnUnauthorized_WhenNotPassingAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithContent(
                    GenerateCreateRequest())
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task CreatePackaging_ShouldReturnForbidden_WhenPassingMemberAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyMember)
                .WithContent(
                    GenerateCreateRequest())
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task CreatePackaging_ShouldSucceed_WhenPassingValidData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest())
                .SendAndDeserializeAsync<PackagingResponse>();

        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.PackagingEndpoints.GetPackagings}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<PackagingResponse>();
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.Created);
        createdResponse.responseContent.ShouldNotBeNull();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Id.ShouldBe(createdResponse.responseContent!.Id);
        getResponse.responseContent.Code.ShouldBe(createdResponse.responseContent!.Code);
        getResponse.responseContent.DepositorCode.ShouldBe(_fixture.ApiSeedData.DepositorCode);
        getResponse.responseContent.PackagingTypeCode.ShouldBe(_fixture.ApiSeedData.PackagingTypeCode);
        getResponse.responseContent.PackagingCirculationCode.ShouldBe(_fixture.ApiSeedData.PackagingCirculationCode);
        getResponse.responseContent.Name.ShouldBe(createdResponse.responseContent!.Name);
        getResponse.responseContent.CustomerNumber.ShouldBe(createdResponse.responseContent!.CustomerNumber);
        getResponse.responseContent.Dimensions.Depth.ShouldBe(createdResponse.responseContent!.Dimensions.Depth);
        getResponse.responseContent.Dimensions.Width.ShouldBe(createdResponse.responseContent!.Dimensions.Width);
        getResponse.responseContent.Dimensions.Height.ShouldBe(createdResponse.responseContent!.Dimensions.Height);
        getResponse.responseContent.Weight.ShouldBe(createdResponse.responseContent!.Weight);
        getResponse.responseContent.MustBeWashed.ShouldBe(createdResponse.responseContent!.MustBeWashed);
    }
    
    [Fact]
    public async Task CreatePackaging_ShouldFail_WhenPassingNonExistingDepositor()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(depositorCode: string.Empty))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }
    
    [Fact]
    public async Task CreatePackaging_ShouldFail_WhenPassingNonExistingPackagingType()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(packagingTypeCode: string.Empty))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }

    [Fact]
    public async Task CreatePackaging_ShouldFail_WhenPassingNonExistingPackagingCirculation()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(packagingCirculationCode: string.Empty))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }
    
    [Fact]
    public async Task CreatePackaging_ShouldFail_WhenPassingEmptyCustomerNumber()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(customerNumber: string.Empty))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }
    
    [Fact]
    public async Task CreatePackaging_ShouldFail_WhenPassingWeight0()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(weight: 0))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }
    
    [Fact]
    public async Task CreatePackaging_ShouldFail_WhenPassingDepth0()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(depth: 0))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }
    
    [Fact]
    public async Task CreatePackaging_ShouldFail_WhenPassingWidth0()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(width: 0))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }
    
    [Fact]
    public async Task CreatePackaging_ShouldFail_WhenPassingHeight0()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(height: 0))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }

    [Fact]
    public async Task CreatePackaging_ShouldSecondFail_WhenPassingSameCombinationOfDepositorIdAndCustomerNumber()
    {
        const string sameCustomerNumber = "123456";
        var firstCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(customerNumber: sameCustomerNumber))
                .SendAsync();
        
        var secondCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(customerNumber: sameCustomerNumber))
                .SendAsync();
        
        secondCreatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreatePackaging_ShouldFail_WhenPassingNonExistingWashingMachineSpeeds()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(
                        washingMachineSpeeds: [
                            new WashingMachineSpeedContract("NoneExistingMachineCode", WashingMachineSpeedLevelContract.Speed1),
                            new WashingMachineSpeedContract(_fixture.ApiSeedData.WashingMachineCode2, WashingMachineSpeedLevelContract.Speed1),
                            new WashingMachineSpeedContract(_fixture.ApiSeedData.WashingMachineCode3, WashingMachineSpeedLevelContract.Speed1)
                        ]))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        createdResponse.responseContent.Errors.First().Slug.ShouldBe(PackagingErrors.ValidationPackagingSpeedsCountMismatch(string.Empty).Code);
    }
    
    [Fact]
    public async Task CreatePackaging_ShouldFail_WhenPassingLessWashingMachineSpeeds()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(
                        washingMachineSpeeds: [
                            new WashingMachineSpeedContract(_fixture.ApiSeedData.WashingMachineCode1, WashingMachineSpeedLevelContract.Speed1),
                            new WashingMachineSpeedContract(_fixture.ApiSeedData.WashingMachineCode2, WashingMachineSpeedLevelContract.Speed1)
                        ]))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        createdResponse.responseContent.Errors.First().Slug.ShouldBe(PackagingErrors.ValidationPackagingSpeedsCountMismatch(string.Empty).Code);
    }
    
    [Fact]
    public async Task CreatePackaging_ShouldFail_WhenPassingMoreWashingMachineSpeeds()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(
                        washingMachineSpeeds: [
                            new WashingMachineSpeedContract(_fixture.ApiSeedData.WashingMachineCode1, WashingMachineSpeedLevelContract.Speed1),
                            new WashingMachineSpeedContract(_fixture.ApiSeedData.WashingMachineCode2, WashingMachineSpeedLevelContract.Speed1),
                            new WashingMachineSpeedContract(_fixture.ApiSeedData.WashingMachineCode3, WashingMachineSpeedLevelContract.Speed1),
                            new WashingMachineSpeedContract("OneMoreWashingMachine", WashingMachineSpeedLevelContract.Speed1)
                        ]))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        createdResponse.responseContent.Errors.First().Slug.ShouldBe(PackagingErrors.ValidationPackagingSpeedsCountMismatch(string.Empty).Code);
    }
    
    [Fact]
    public async Task GetPackagings_ShouldReturnSomething_WhenAtLeastOnePackagingCreated()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.PackagingEndpoints.GetPackagings}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<PackagingResponse>>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.GetType().ShouldBe(typeof(FilteredResult<PackagingResponse>));
        getResponse.responseContent.Data.Count.ShouldBeGreaterThan(0);
    }
    
    [Fact]
    public async Task GetGroupedPackagings_ShouldReturnCodes_WhenGroupedCodeIsRequested()
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.PackagingEndpoints.GetPackagings}?group=[{{\"selector\":\"code\",\"isExpanded\":false}}]")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        var content = await getResponse.Content.ReadAsStringAsync();
        
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        content.ShouldContain($"{{\"key\":\"{_fixture.ApiSeedData.PackagingCode}\"}}");
        content.ShouldContain($"{{\"key\":\"{_fixture.ApiSeedData.PackagingCode2}\"}}");
        content.ShouldContain($"{{\"key\":\"{_fixture.ApiSeedData.PackagingCode3}\"}}");
    }
    
    [Fact]
    public async Task DeletePackaging_ShouldSucceed_WhenPackagingDeleted()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<PackagingResponse>();
        
        var getResponseBeforeDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.PackagingEndpoints.GetPackagings}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<PackagingResponse>();
        
        var deleteResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete, 
                    $"{SettingsEndpoints.PackagingEndpoints.GetPackagings}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        var getResponseAfterDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.PackagingEndpoints.GetPackagings}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<PackagingResponse>(ensureSuccessStatusCode: false);
        
        getResponseBeforeDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponseBeforeDelete.responseContent.ShouldNotBeNull();
        
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        
        getResponseAfterDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeletePackaging_ShouldReturnNotFound_WhenPassingNonExistingCode()
    {
        var deleteResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete, 
                    $"{SettingsEndpoints.PackagingEndpoints.GetPackagings}/{Guid.NewGuid()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        deleteResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        deleteResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        deleteResponse.responseContent!.Errors.ShouldContain(x => x.Slug == PackagingErrors.CodeDoesNotExist.Code);
        deleteResponse.responseContent!.Errors.ShouldContain(x => x.Message == PackagingErrors.CodeDoesNotExist.Description);
    }
    
    [Fact]
    public async Task DeletePackaging_ShouldReturnUnprocessableEntity_WhenCodeExistsInExistingKit()
    {
        var deleteResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete, 
                    $"{SettingsEndpoints.PackagingEndpoints.GetPackagings}/{_fixture.ApiSeedData.PackagingCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        deleteResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        deleteResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        deleteResponse.responseContent!.Errors.ShouldContain(x => x.Slug == PackagingErrors.ValidationPackagingIsUsedInKits(_fixture.ApiSeedData.KitCode).Code);
    }
    
    [Fact]
    public async Task UpdatePackaging_ShouldSucceed_WhenPassingValidData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<PackagingResponse>();

        var updatePackagingBody = GenerateUpdateRequest();
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.PackagingEndpoints.GetPackagings}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updatePackagingBody)
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.PackagingEndpoints.GetPackagings}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<PackagingResponse>();
        
        getResponse.responseContent!.PackagingTypeCode.ShouldBe(updatePackagingBody.PackagingTypeCode);
        getResponse.responseContent!.PackagingCirculationCode.ShouldBe(updatePackagingBody.PackagingCirculationCode);
        getResponse.responseContent!.Name.ShouldBe(updatePackagingBody.Name);
        getResponse.responseContent!.MustBeWashed.ShouldBe(updatePackagingBody.MustBeWashed);
        getResponse.responseContent!.Weight.ShouldBe(updatePackagingBody.Weight);
        getResponse.responseContent!.Dimensions.Depth.ShouldBe(updatePackagingBody.Dimensions.Depth);
        getResponse.responseContent!.Dimensions.Width.ShouldBe(updatePackagingBody.Dimensions.Width);
        getResponse.responseContent!.Dimensions.Height.ShouldBe(updatePackagingBody.Dimensions.Height);
    }
    
    [Fact]
    public async Task UpdatePackaging_ShouldReturnNotFound_WhenPassingNonExistingToken()
    {
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.PackagingEndpoints.GetPackagings}/{Guid.NewGuid()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateUpdateRequest())
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdatePackaging_ShouldFail_WhenPassingNonExistingPackagingTypeCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<PackagingResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.PackagingEndpoints.GetPackagings}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateUpdateRequest(packagingTypeCode: "NonExistingPackagingType"))
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task UpdatePackaging_ShouldFail_WhenPassingNonExistingPackagingCirculationCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<PackagingResponse>();

        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.PackagingEndpoints.GetPackagings}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateUpdateRequest(packagingCirculationCode: "NonExistingPackagingCirculationCode"))
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task UpdatePackaging_ShouldFail_WhenPassingInvalidWeight()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<PackagingResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.PackagingEndpoints.GetPackagings}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateUpdateRequest(weight: 0))
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task UpdatePackaging_ShouldFail_WhenPassingInvalidDimensions()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<PackagingResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.PackagingEndpoints.GetPackagings}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateUpdateRequest(dimensionsContract: new DimensionsContract(Depth: -1, Width: 0, Height: 300)))
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task UpdatePackaging_ShouldFail_WhenPassingInvalidWashingMachineSpeeds()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<PackagingResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.PackagingEndpoints.GetPackagings}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateUpdateRequest(
                    washingMachineSpeeds: [
                        new WashingMachineSpeedContract("NonExistingWashingMachine", WashingMachineSpeedLevelContract.Speed1),
                        new WashingMachineSpeedContract(_fixture.ApiSeedData.WashingMachineCode2, WashingMachineSpeedLevelContract.Speed3),
                        new WashingMachineSpeedContract(_fixture.ApiSeedData.WashingMachineCode3, WashingMachineSpeedLevelContract.NotSet),
                        new WashingMachineSpeedContract("OneMoreWashingMachine", WashingMachineSpeedLevelContract.Speed1)
                    ]))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        updatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updatedResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        updatedResponse.responseContent.Errors.First().Slug.ShouldBe(PackagingErrors.ValidationPackagingSpeedsCountMismatch(string.Empty).Code);
    }
    
    private UpdatePackagingRequest GenerateUpdateRequest(
        string? packagingTypeCode = null,
        string? packagingCirculationCode = null,
        int? weight = null,
        DimensionsContract? dimensionsContract = null,
        WashingMachineSpeedContract[]? washingMachineSpeeds = null) => new (
        PackagingTypeCode: packagingTypeCode ?? _fixture.ApiSeedData.PackagingTypeCode,
        PackagingCirculationCode: packagingCirculationCode ?? _fixture.ApiSeedData.PackagingCirculationCode,
        Name: "Packaging 5",
        MustBeWashed: false,
        Dimensions: dimensionsContract ?? new DimensionsContract(Depth: 100, Width: 200, Height: 300),
        Weight: weight ?? 50,
        WashingMachineSpeeds: washingMachineSpeeds ??
        [
            new WashingMachineSpeedContract(_fixture.ApiSeedData.WashingMachineCode1, WashingMachineSpeedLevelContract.Speed3),
            new WashingMachineSpeedContract(_fixture.ApiSeedData.WashingMachineCode2, WashingMachineSpeedLevelContract.Speed2),
            new WashingMachineSpeedContract(_fixture.ApiSeedData.WashingMachineCode3, WashingMachineSpeedLevelContract.Speed1)
        ]
    );
    
    public CreatePackagingRequest GenerateCreateRequest(
        string? packagingTypeCode = null,
        string? depositorCode = null,
        string? packagingCirculationCode = null,
        string? customerNumber = null,
        int? weight = null,
        int? depth = null,
        int? width = null,
        int? height = null,
        WashingMachineSpeedContract[]? washingMachineSpeeds = null) => new (
        PackagingTypeCode: packagingTypeCode ?? _fixture.ApiSeedData.PackagingTypeCode,
        DepositorCode: depositorCode ?? _fixture.ApiSeedData.DepositorCode,
        PackagingCirculationCode: packagingCirculationCode ?? _fixture.ApiSeedData.PackagingCirculationCode,
        CustomerNumber: customerNumber ?? new Random().Next(100000, 1000000).ToString(),
        Name: "Packaging 1",
        MustBeWashed: true,
        Dimensions: new DimensionsContract(depth ?? 10, width ?? 20, height ?? 30),
        Weight: weight ?? 5,
        WashingMachineSpeeds: washingMachineSpeeds ??
        [
            new WashingMachineSpeedContract(_fixture.ApiSeedData.WashingMachineCode1, WashingMachineSpeedLevelContract.Speed1),
            new WashingMachineSpeedContract(_fixture.ApiSeedData.WashingMachineCode2, WashingMachineSpeedLevelContract.Speed2),
            new WashingMachineSpeedContract(_fixture.ApiSeedData.WashingMachineCode3, WashingMachineSpeedLevelContract.Speed3)
        ]
    );
}