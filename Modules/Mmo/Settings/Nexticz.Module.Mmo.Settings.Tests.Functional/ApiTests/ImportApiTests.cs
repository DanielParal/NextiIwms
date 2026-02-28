using System.Net;
using Nexticz.Module.Mmo.Settings.Application.ImportsExports.Imports;
using Nexticz.Module.Mmo.Settings.Contracts.Imports;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;
using Nexticz.Module.Mmo.Settings.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Lib.Shared.DevExtreme;

using Shouldly;

namespace Nexticz.Module.Mmo.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class ImportApiTests
{
    private readonly HttpClient _client;

    public ImportApiTests(SettingsApiFactoryFixture fixture)
    {
        _client = fixture.Factory.CreateClient();
    }
    
    [Fact]
    public async Task CreateImport_ShouldReturnUnauthorized_WhenNotPassingAuthToken()
    {
        var multipartFormDataBuilder = await new MultipartFormDataBuilder()
            .WithStringData("ImportType", nameof(ImportType.PackagingXlsx))
            .WithFileAsync("PackagingUpload.xlsx", FileType.Xlsx);

        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ImportEndpoints.CreateImport)
                .WithMultipartFormData(multipartFormDataBuilder.Build())
                .SendAsync(); 
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task CreateImport_ShouldReturnForbidden_WhenPassingMemberAuthToken()
    {
        var multipartFormDataBuilder = await new MultipartFormDataBuilder()
            .WithStringData("ImportType", nameof(ImportType.PackagingXlsx))
            .WithFileAsync("PackagingUpload.xlsx", FileType.Xlsx);

        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ImportEndpoints.CreateImport)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyMember)
                .WithMultipartFormData(multipartFormDataBuilder.Build())
                .SendAsync(); 
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
    
    // We need to run those two tests together because kit packaging import contains data which are needed for kit import
    [Fact]
    public async Task CreatePackagingAndKitImport_ShouldBothPartiallySucceedWithErrors_WhenExcelContainsValidDataWithErrors()
    {
        var packagingMultipartFormDataBuilder = await new MultipartFormDataBuilder()
            .WithStringData("ImportType", nameof(ImportType.PackagingXlsx))
            .WithFileAsync("PackagingUpload.xlsx", FileType.Xlsx);

        var packagingCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ImportEndpoints.CreateImport)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithMultipartFormData(packagingMultipartFormDataBuilder.Build())
                .SendAndDeserializeAsync<ImportResponse>(); 
        
        packagingCreatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        packagingCreatedResponse.responseContent.ShouldNotBeNull();
        packagingCreatedResponse.responseContent.GetType().ShouldBe(typeof(ImportResponse));
        packagingCreatedResponse.responseContent.Status.ShouldBe(ImportStatusContract.PartialSuccessWithErrors);
        packagingCreatedResponse.responseContent.ImportedCodesCount.ShouldBeGreaterThan(0);
        packagingCreatedResponse.responseContent.Errors.ShouldContain(
            x => 
                x.Code.Contains("D0016000101178") || 
                x.Code.Contains("D0016000101179") ||
                x.Code.Contains("NONEXISTING6000101176") ||
                x.Code.Contains("D0016000101177") ||
                x.Code.Contains("D0016000101180") ||
                x.Code.Contains("D0016000101181") ||
                x.Code.Contains("D0016000101183") ||
                x.Code.Contains("D0016000101184")
                );
        
        
        var kitMultipartFormDataBuilder = await new MultipartFormDataBuilder()
            .WithStringData("ImportType", nameof(ImportType.KitXlsx))
            .WithFileAsync("KitUpload.xlsx", FileType.Xlsx);

        var kitCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ImportEndpoints.CreateImport)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithMultipartFormData(kitMultipartFormDataBuilder.Build())
                .SendAndDeserializeAsync<ImportResponse>(); 
        
        kitCreatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        kitCreatedResponse.responseContent.ShouldNotBeNull();
        kitCreatedResponse.responseContent.GetType().ShouldBe(typeof(ImportResponse));
        kitCreatedResponse.responseContent.Status.ShouldBe(ImportStatusContract.PartialSuccessWithErrors);
        kitCreatedResponse.responseContent.ImportedCodesCount.ShouldBeGreaterThan(0);
        
        kitCreatedResponse.responseContent.Errors.ShouldContain(
            x => 
                x.Code.Contains("D001KT0016000000096") || 
                x.Code.Contains("D001KT0016000000097") ||
                x.Code.Contains("D001KT0016000000101") ||
                x.Code.Contains("D001KT0016000000098")
        );
    }
    
    [Fact]
    public async Task CreatePackagingAndKitImport_ShouldBothFail_WhenPassingEmptyExcels()
    {
        var packagingMultipartFormDataBuilder = await new MultipartFormDataBuilder()
            .WithStringData("ImportType", nameof(ImportType.PackagingXlsx))
            .WithFileAsync("PackagingUploadEmpty.xlsx", FileType.Xlsx);

        var packagingCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ImportEndpoints.CreateImport)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithMultipartFormData(packagingMultipartFormDataBuilder.Build())
                .SendAndDeserializeAsync<ImportResponse>(); 
        
        packagingCreatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        packagingCreatedResponse.responseContent.ShouldNotBeNull();
        packagingCreatedResponse.responseContent.GetType().ShouldBe(typeof(ImportResponse));
        packagingCreatedResponse.responseContent.Status.ShouldBe(ImportStatusContract.Failure);
        packagingCreatedResponse.responseContent.Type.ShouldBe(ImportTypeContract.PackagingXlsx);
        packagingCreatedResponse.responseContent.ImportedCodesCount.ShouldBe(0);
        packagingCreatedResponse.responseContent.Errors.Length.ShouldBe(1);
        packagingCreatedResponse.responseContent.Errors.First().Code.ShouldBe(Lib.Shared.ImportsExports.Imports.ImportErrors.ValidationFileHasNoData.Code);
        
        var kitMultipartFormDataBuilder = await new MultipartFormDataBuilder()
            .WithStringData("ImportType", nameof(ImportType.KitXlsx))
            .WithFileAsync("KitUploadEmpty.xlsx", FileType.Xlsx);

        var kitCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ImportEndpoints.CreateImport)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithMultipartFormData(kitMultipartFormDataBuilder.Build())
                .SendAndDeserializeAsync<ImportResponse>(); 
        
        kitCreatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        kitCreatedResponse.responseContent.ShouldNotBeNull();
        kitCreatedResponse.responseContent.GetType().ShouldBe(typeof(ImportResponse));
        kitCreatedResponse.responseContent.Status.ShouldBe(ImportStatusContract.Failure);
        kitCreatedResponse.responseContent.Type.ShouldBe(ImportTypeContract.KitXlsx);
        kitCreatedResponse.responseContent.ImportedCodesCount.ShouldBe(0);
        kitCreatedResponse.responseContent.Errors.Length.ShouldBe(1);
        kitCreatedResponse.responseContent.Errors.First().Code.ShouldBe(Lib.Shared.ImportsExports.Imports.ImportErrors.ValidationFileHasNoData.Code);
    }
    
    [Fact]
    public async Task CreatePackagingAndKitImport_ShouldBothFail_WhenPassingNotExcelFile()
    {
        var packagingMultipartFormDataBuilder = await new MultipartFormDataBuilder()
            .WithStringData("ImportType", nameof(ImportType.PackagingXlsx))
            .WithFileAsync("NotExcel.txt", FileType.Txt);

        var packagingCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ImportEndpoints.CreateImport)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithMultipartFormData(packagingMultipartFormDataBuilder.Build())
                .SendAndDeserializeAsync<ImportResponse>(); 
        
        packagingCreatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        packagingCreatedResponse.responseContent.ShouldNotBeNull();
        packagingCreatedResponse.responseContent.GetType().ShouldBe(typeof(ImportResponse));
        packagingCreatedResponse.responseContent.Status.ShouldBe(ImportStatusContract.Failure);
        packagingCreatedResponse.responseContent.Type.ShouldBe(ImportTypeContract.PackagingXlsx);
        packagingCreatedResponse.responseContent.ImportedCodesCount.ShouldBe(0);
        packagingCreatedResponse.responseContent.Errors.Length.ShouldBe(1);
        packagingCreatedResponse.responseContent.Errors.First().Code.ShouldBe(Lib.Shared.ImportsExports.Imports.ImportErrors.ValidationFileIsNotExcel.Code);
        
        var kitMultipartFormDataBuilder = await new MultipartFormDataBuilder()
            .WithStringData("ImportType", nameof(ImportType.KitXlsx))
            .WithFileAsync("NotExcel.txt", FileType.Txt);

        var kitCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ImportEndpoints.CreateImport)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithMultipartFormData(kitMultipartFormDataBuilder.Build())
                .SendAndDeserializeAsync<ImportResponse>(); 
        
        kitCreatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        kitCreatedResponse.responseContent.ShouldNotBeNull();
        kitCreatedResponse.responseContent.GetType().ShouldBe(typeof(ImportResponse));
        kitCreatedResponse.responseContent.Status.ShouldBe(ImportStatusContract.Failure);
        kitCreatedResponse.responseContent.Type.ShouldBe(ImportTypeContract.KitXlsx);
        kitCreatedResponse.responseContent.ImportedCodesCount.ShouldBe(0);
        kitCreatedResponse.responseContent.Errors.Length.ShouldBe(1);
        kitCreatedResponse.responseContent.Errors.First().Code.ShouldBe(Lib.Shared.ImportsExports.Imports.ImportErrors.ValidationFileIsNotExcel.Code);
    }
    
    [Fact]
    public async Task CreatePackagingAndKitImport_ShouldBothFail_WhenExcelHasWrongColumns()
    {
        var packagingMultipartFormDataBuilder = await new MultipartFormDataBuilder()
            .WithStringData("ImportType", nameof(ImportType.PackagingXlsx))
            .WithFileAsync("PackagingUploadWrongColumns.xlsx", FileType.Xlsx);

        var packagingCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ImportEndpoints.CreateImport)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithMultipartFormData(packagingMultipartFormDataBuilder.Build())
                .SendAndDeserializeAsync<ImportResponse>(); 
        
        packagingCreatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        packagingCreatedResponse.responseContent.ShouldNotBeNull();
        packagingCreatedResponse.responseContent.GetType().ShouldBe(typeof(ImportResponse));
        packagingCreatedResponse.responseContent.Status.ShouldBe(ImportStatusContract.Failure);
        packagingCreatedResponse.responseContent.Type.ShouldBe(ImportTypeContract.PackagingXlsx);
        packagingCreatedResponse.responseContent.ImportedCodesCount.ShouldBe(0);
        packagingCreatedResponse.responseContent.Errors.Length.ShouldBe(1);
        packagingCreatedResponse.responseContent.Errors.First().Code.ShouldBe(Lib.Shared.ImportsExports.Imports.ImportErrors.ValidationFileHasWrongFormat(string.Empty).Code);
        packagingCreatedResponse.responseContent.Errors.First().Message.ShouldContain("ZakaznickeCisloObalu, KodUkladatele, KodTypuObalu, KodObehovostiObalu, NutnoPrat, NazevObalu, Hloubka, Sirka, Vyska, Hmotnost");
        
        var kitMultipartFormDataBuilder = await new MultipartFormDataBuilder()
            .WithStringData("ImportType", nameof(ImportType.KitXlsx))
            .WithFileAsync("KitUploadWrongColumns.xlsx", FileType.Xlsx);

        var kitCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ImportEndpoints.CreateImport)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithMultipartFormData(kitMultipartFormDataBuilder.Build())
                .SendAndDeserializeAsync<ImportResponse>(); 
        
        kitCreatedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        kitCreatedResponse.responseContent.ShouldNotBeNull();
        kitCreatedResponse.responseContent.GetType().ShouldBe(typeof(ImportResponse));
        kitCreatedResponse.responseContent.Status.ShouldBe(ImportStatusContract.Failure);
        kitCreatedResponse.responseContent.Type.ShouldBe(ImportTypeContract.KitXlsx);
        kitCreatedResponse.responseContent.ImportedCodesCount.ShouldBe(0);
        kitCreatedResponse.responseContent.Errors.Length.ShouldBe(1);
        kitCreatedResponse.responseContent.Errors.First().Code.ShouldBe(Lib.Shared.ImportsExports.Imports.ImportErrors.ValidationFileHasWrongFormat(string.Empty).Code);
        kitCreatedResponse.responseContent.Errors.First().Message.ShouldContain("CisloKitu, KodUkladatele, KodTypuKitu, KodDefiniceSapuKitu, KodVyroby, KodUrcujicihoBaleni, KodBaleni, PocetBaleniVKitu, Poznamka, CasChladnuti, ObsahujeBaliciPredpis");
    }
    
    [Fact]
    public async Task GetImports_ShouldReturnSomething_WhenAtLeastOneImportCreated()
    {
        var multipartFormDataBuilder = await new MultipartFormDataBuilder()
            .WithStringData("ImportType", nameof(ImportType.PackagingXlsx))
            .WithFileAsync("PackagingUpload.xlsx", FileType.Xlsx);

        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ImportEndpoints.CreateImport)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithMultipartFormData(multipartFormDataBuilder.Build())
                .SendAsync(); 
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.ImportEndpoints.GetImports}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<ImportResponse>>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.GetType().ShouldBe(typeof(FilteredResult<ImportResponse>));
        getResponse.responseContent.Data.Count.ShouldBeGreaterThan(0);
    }
}