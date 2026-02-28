using System.Net;
using Nexticz.Module.Mmo.Settings.Application.Kits;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Module.Mmo.Settings.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Errors.Models;

using Shouldly;

namespace Nexticz.Module.Mmo.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class KitApiTests
{
    private readonly SettingsApiFactoryFixture _fixture;
    private readonly HttpClient _client;
    
    public KitApiTests(SettingsApiFactoryFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Factory.CreateClient();
    }
    
    [Fact]
    public async Task CreateKit_ShouldReturnUnauthorized_WhenNotPassingAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithContent(
                    GenerateCreateRequest())
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task CreateKit_ShouldReturnForbidden_WhenPassingMemberAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyMember)
                .WithContent(
                    GenerateCreateRequest())
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task CreateKit_ShouldSucceed_WhenPassingValidData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest())
                .SendAndDeserializeAsync<KitResponse>();

        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.KitEndpoints.GetKits}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<KitResponse>();
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.Created);
        createdResponse.responseContent.ShouldNotBeNull();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Id.ShouldBe(createdResponse.responseContent!.Id);
        getResponse.responseContent.Code.ShouldBe(createdResponse.responseContent.Code);
        getResponse.responseContent.DepositorCode.ShouldBe(_fixture.ApiSeedData.DepositorCode);
        getResponse.responseContent.KitTypeCode.ShouldBe(_fixture.ApiSeedData.KitTypeCode);
        getResponse.responseContent.ManufactureCode.ShouldBe(_fixture.ApiSeedData.ManufactureCode);
        getResponse.responseContent.KitNumber.ShouldBe(createdResponse.responseContent!.KitNumber);
        getResponse.responseContent.Note.ShouldBe(createdResponse.responseContent!.Note);
        getResponse.responseContent.DefiningPackagingCode.ShouldBe(createdResponse.responseContent!.DefiningPackagingCode);
        getResponse.responseContent.DryingTime.ShouldBe(createdResponse.responseContent!.DryingTime);
    }
    
    [Fact]
    public async Task CreateKit_ShouldFail_WhenPassingNonExistingDepositor()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(depositorCode: string.Empty))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }
    
    [Fact]
    public async Task CreateKit_ShouldFail_WhenPassingNonExistingKitType()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(kitTypeCode: string.Empty))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }

    [Fact]
    public async Task CreateKit_ShouldFail_WhenPassingNonExistingManufacture()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(manufactureCode: string.Empty))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }
    
    [Fact]
    public async Task CreateKit_ShouldFail_WhenPassingNotValidKitNumber0()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(kitNumber: string.Empty))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }
    
    [Fact]
    public async Task CreateKit_ShouldFail_WhenPassingIncorrectDefiningPackagingNumber()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(definingPackagingCode: string.Empty))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }
    
    [Fact]
    public async Task CreateKit_ShouldFail_WhenPassingDryingTime0()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(dryingTime: 0))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
    }

    [Fact]
    public async Task CreateKit_ShouldSecondFail_WhenPassingSameCombinationOfDepositorIdAndKitNumber()
    {
        const string sameKitNumber = "123456";
        var firstCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(kitNumber: sameKitNumber))
                .SendAsync();
        
        var secondCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(kitNumber: sameKitNumber))
                .SendAsync();
        
        secondCreatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateKit_ShouldFail_WhenPassingEmptyCodes()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(packagingCodeQuantities: []))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        createdResponse.responseContent!.Errors.ShouldContain(x => x.Slug == KitErrors.ValidationPackagingCodeQuantitiesIsRequired.Code);
    }
    
    [Fact]
    public async Task CreateKit_ShouldFail_WhenPassingDefiningPackagingCodeIsNotWithinTheListOfCodes()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(definingPackagingCode: "DefiningPackagingCodeNotInTheListOfCodes"))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        createdResponse.responseContent!.Errors.ShouldContain(x => x.Slug == KitErrors.ValidationDefiningPackagingCodeNotIncludedInPackagingCodes.Code);
    }
    
    [Fact]
    public async Task CreateKit_ShouldFail_WhenPassingNonExistingPackagingCodes()
    {
        const string nonExistingPackagingCode = "NonExistingPackagingCode";
        const string nonExistingPackagingCode2 = "NonExistingPackagingCode2";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(
                        packagingCodeQuantities: 
                        [
                            new PackagingQuantityContract(nonExistingPackagingCode, string.Empty, 3),
                            new PackagingQuantityContract(nonExistingPackagingCode2, string.Empty, 1),
                            new PackagingQuantityContract(_fixture.ApiSeedData.PackagingCode, string.Empty, 15),
                            new PackagingQuantityContract(_fixture.ApiSeedData.PackagingCode2, string.Empty, 5)
                        ]))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);

        createdResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        createdResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        createdResponse.responseContent!.Errors.ShouldContain(x => x.Slug == KitErrors.ValidationMissingPackagingCodes($"{nonExistingPackagingCode}, {nonExistingPackagingCode2}").Code);
        createdResponse.responseContent!.Errors.ShouldContain(x => x.Message == KitErrors.ValidationMissingPackagingCodes($"{nonExistingPackagingCode.ToUpperInvariant()}, {nonExistingPackagingCode2.ToUpperInvariant()}").Description);
    }
    
    [Fact]
    public async Task GetKits_ShouldReturnSomething_WhenAtLeastOneKitCreated()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.KitEndpoints.GetKits}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<KitResponse>>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.GetType().ShouldBe(typeof(FilteredResult<KitResponse>));
        getResponse.responseContent.Data.Count.ShouldBeGreaterThan(0);
    }
    
    [Fact]
    public async Task DeleteKit_ShouldNotFound_WhenKitDeleted()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<KitResponse>();
        
        var getResponseBeforeDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.KitEndpoints.GetKits}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<KitResponse>();
        
        var deleteResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Delete, 
                    $"{SettingsEndpoints.KitEndpoints.GetKits}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        var getResponseAfterDelete = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.KitEndpoints.GetKits}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<KitResponse>(ensureSuccessStatusCode: false);
        
        getResponseBeforeDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponseBeforeDelete.responseContent.ShouldNotBeNull();
        
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        
        getResponseAfterDelete.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdateKit_ShouldSucceed_WhenPassingValidData()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<KitResponse>();

        var updateKitBody = GenerateUpdateRequest();
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.KitEndpoints.GetKits}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateKitBody)
                .SendAsync();
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.KitEndpoints.GetKits}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<KitResponse>();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        
        getResponse.responseContent!.Code.ShouldBe(createdResponse.responseContent!.Code);
        getResponse.responseContent.DepositorCode.ShouldBe(createdResponse.responseContent.DepositorCode);
        getResponse.responseContent.KitNumber.ShouldBe(createdResponse.responseContent!.KitNumber);
        
        getResponse.responseContent.ManufactureCode.ShouldBe(updateKitBody.ManufactureCode);
        getResponse.responseContent.Note.ShouldBe(updateKitBody.Note);
        getResponse.responseContent.DefiningPackagingCode.ShouldBe(updateKitBody.DefiningPackagingCode);
        getResponse.responseContent.DryingTime.ShouldBe(updateKitBody.DryingTime);
        getResponse.responseContent.PackagingQuantities.ShouldBe(updateKitBody.PackagingQuantities);
    }
    
    [Fact]
    public async Task UpdateKit_ShouldReturnNotFound_WhenPassingNonExistingToken()
    {
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.KitEndpoints.GetKits}/{Guid.NewGuid()}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdateKit_ShouldFail_WhenPassingNonExistingManufactureCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<KitResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.KitEndpoints.GetKits}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateUpdateRequest(manufactureCode: string.Empty))
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task UpdateKit_ShouldFail_WhenPassingNonExistingKitSapDefinitionCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<KitResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.KitEndpoints.GetKits}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateUpdateRequest(kitSapDefinitionCode: string.Empty))
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task UpdateKit_ShouldFail_WhenPassingInvalidDryingTime()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<KitResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.KitEndpoints.GetKits}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateUpdateRequest(dryingTime: 0))
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task UpdateKit_ShouldFail_WhenPassingInvalidDefiningPackagingNumber()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateCreateRequest())
                .SendAndDeserializeAsync<KitResponse>();
        
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.KitEndpoints.GetKits}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateUpdateRequest(definingPackagingCode: string.Empty))
                .SendAsync();
        
        updatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task UpdateKit_ShouldFail_WhenPassingEmptyCodes()
    {
        var updatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.KitEndpoints.GetKits}/{_fixture.ApiSeedData.KitCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(GenerateUpdateRequest(packagingCodeQuantities: []))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        updatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updatedResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        updatedResponse.responseContent!.Errors.ShouldContain(x => x.Slug == KitErrors.ValidationPackagingCodeQuantitiesIsRequired.Code);
    }
    
    [Fact]
    public async Task UpdateKit_ShouldFail_WhenPassingDefiningPackagingCodeIsNotWithinTheListOfCodes()
    {
        var updateResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.KitEndpoints.GetKits}/{_fixture.ApiSeedData.KitCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateUpdateRequest(definingPackagingCode: "DefiningPackagingCodeNotInTheListOfCodes"))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
    
        updateResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updateResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        updateResponse.responseContent!.Errors.ShouldContain(x => x.Slug == KitErrors.ValidationDefiningPackagingCodeNotIncludedInPackagingCodes.Code);
    }
    
    [Fact]
    public async Task UpdateKit_ShouldFail_WhenPassingNonExistingPackagingCodes()
    {
        const string nonExistingPackagingCode = "NonExistingPackagingCode";
        const string nonExistingPackagingCode2 = "NonExistingPackagingCode2";
        var updateResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Put,$"{SettingsEndpoints.KitEndpoints.GetKits}/{_fixture.ApiSeedData.KitCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateCreateRequest(
                        packagingCodeQuantities: 
                        [
                            new PackagingQuantityContract(nonExistingPackagingCode, string.Empty, 3),
                            new PackagingQuantityContract(nonExistingPackagingCode2, string.Empty, 1),
                            new PackagingQuantityContract(_fixture.ApiSeedData.PackagingCode, _fixture.ApiSeedData.PackagingTypeName, 15),
                            new PackagingQuantityContract(_fixture.ApiSeedData.PackagingCode2, _fixture.ApiSeedData.PackagingTypeName, 5)
                        ]))
                .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
    
        updateResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updateResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        updateResponse.responseContent!.Errors.ShouldContain(x => x.Slug == KitErrors.ValidationMissingPackagingCodes($"{nonExistingPackagingCode}, {nonExistingPackagingCode2}").Code);
        updateResponse.responseContent!.Errors.ShouldContain(x => x.Message == KitErrors.ValidationMissingPackagingCodes($"{nonExistingPackagingCode.ToUpperInvariant()}, {nonExistingPackagingCode2.ToUpperInvariant()}").Description);
    }
    
    private UpdateKitRequest GenerateUpdateRequest(
        string? manufactureCode = null,
        string? kitSapDefinitionCode = null,
        string? definingPackagingCode = null,
        int? dryingTime = null,
        PackagingQuantityContract[]? packagingCodeQuantities = null) => new (
        ManufactureCode: manufactureCode ?? _fixture.ApiSeedData.ManufactureCode,
        KitSapDefinitionCode: kitSapDefinitionCode ?? _fixture.ApiSeedData.KitSapDefinitionCode,
        Note: "Kit 1 update",
        DefiningPackagingCode: definingPackagingCode ?? _fixture.ApiSeedData.KitDefiningPackagingNumber,
        DryingTime: dryingTime ?? 15,
        PackagingQuantities: packagingCodeQuantities ?? 
                        [
                            new PackagingQuantityContract(_fixture.ApiSeedData.PackagingCode, _fixture.ApiSeedData.PackagingTypeName, 3),
                            new PackagingQuantityContract(_fixture.ApiSeedData.PackagingCode2, _fixture.ApiSeedData.PackagingTypeName, 1)
                        ],
        SpecialInformationSchedules: []
    );
    
    private CreateKitRequest GenerateCreateRequest(
        string? kitTypeCode = null,
        string? kitSapDefinitionCode = null,
        string? depositorCode = null,
        string? manufactureCode = null,
        string? kitNumber = null,
        string? definingPackagingCode = null,
        int? dryingTime = null,
        PackagingQuantityContract[]? packagingCodeQuantities = null) => new (
        KitTypeCode: kitTypeCode ?? _fixture.ApiSeedData.KitTypeCode,
        KitSapDefinitionCode: kitSapDefinitionCode ?? _fixture.ApiSeedData.KitSapDefinitionCode,
        DepositorCode: depositorCode ?? _fixture.ApiSeedData.DepositorCode,
        ManufactureCode: manufactureCode ?? _fixture.ApiSeedData.ManufactureCode,
        KitNumber: kitNumber ?? Guid.NewGuid().ToString(),
        Note: "Kit 1 create",
        DefiningPackagingCode: definingPackagingCode ?? _fixture.ApiSeedData.KitDefiningPackagingNumber,
        DryingTime: dryingTime ?? 5,
        PackagingQuantities: packagingCodeQuantities ?? 
                                 [
                                     new PackagingQuantityContract(_fixture.ApiSeedData.PackagingCode, _fixture.ApiSeedData.PackagingTypeName, 3),
                                     new PackagingQuantityContract(_fixture.ApiSeedData.PackagingCode2, _fixture.ApiSeedData.PackagingTypeName, 1),
                                     new PackagingQuantityContract(_fixture.ApiSeedData.PackagingCode3, _fixture.ApiSeedData.PackagingTypeName, 15)
                                 ],
        SpecialInformationSchedules: []
    );
}