using Nexticz.Module.Mmo.Settings.Contracts.Imports;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;
using Nexticz.Module.Mmo.Settings.Presentation;
using Nexticz.Module.Mmo.SharedTesting;

namespace Nexticz.Module.Mmo.Planning.Tests.Functional;

internal class PlanningDbSeeder
{
    public static async Task SeedDataAsync(HttpClient client)
    {
        await new HttpRequestBuilder(client, HttpMethod.Post, SettingsEndpoints.SeedEndpoints.CreateSeed)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        var packagingMultipartFormDataBuilder = await new MultipartFormDataBuilder()
            .WithStringData("ImportType", ImportType.PackagingXlsx.ToString())
            .WithFileAsync("PackagingUpload.xlsx", FileType.Xlsx);

        var packagingCreatedResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post, SettingsEndpoints.ImportEndpoints.CreateImport)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithMultipartFormData(packagingMultipartFormDataBuilder.Build())
                .SendAndDeserializeAsync<ImportResponse>();
        
        var kitMultipartFormDataBuilder = await new MultipartFormDataBuilder()
            .WithStringData("ImportType", ImportType.KitXlsx.ToString())
            .WithFileAsync("KitUpload.xlsx", FileType.Xlsx);

        var kitCreatedResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post, SettingsEndpoints.ImportEndpoints.CreateImport)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithMultipartFormData(kitMultipartFormDataBuilder.Build())
                .SendAndDeserializeAsync<ImportResponse>();
    }
}