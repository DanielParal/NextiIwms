using Nexticz.Module.Mmo.Settings.Contracts.Imports;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Module.Mmo.Settings.Contracts.SpecialInformations;
using Nexticz.Module.Mmo.Settings.Contracts.Workers;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;
using Nexticz.Module.Mmo.Settings.Presentation;
using Nexticz.Module.Mmo.SharedTesting;

namespace Nexticz.Module.Mmo.FullWorkflowApiTests;

public class FullWorkflowTestsDbSeeder
{
    public static async Task<FullWorkflowApiSeedData> SeedDataAsync(HttpClient client)
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
        
        var createWorker1 = await
            new HttpRequestBuilder(client, HttpMethod.Post,SettingsEndpoints.WorkerEndpoints.CreateWorker)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Pin = 1234,
                    Name = "Worker 1",
                    IsActive = true
                })
                .SendAndDeserializeAsync<WorkerResponse>();
        
        var worker1Pin = createWorker1.responseContent!.Pin;
        
        var createWorker2 = await
            new HttpRequestBuilder(client, HttpMethod.Post,SettingsEndpoints.WorkerEndpoints.CreateWorker)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Pin = 2345,
                    Name = "Worker 2",
                    IsActive = true
                })
                .SendAndDeserializeAsync<WorkerResponse>();
        
        var worker2Pin = createWorker2.responseContent!.Pin;
        
        var createSpecialInformation = await
            new HttpRequestBuilder(client, HttpMethod.Post,SettingsEndpoints.SpecialInformationEndpoints.CreateSpecialInformation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Title = "Special Information 1",
                    Description = "Special Information 1 Description"
                })
                .SendAndDeserializeAsync<SpecialInformationResponse>();
        
        var specialInformationId = createSpecialInformation.responseContent!.Id;

        const string kitCode = "BOZ6000009288";
        var getKitResponse = await
            new HttpRequestBuilder(client, HttpMethod.Get, 
                    $"{SettingsEndpoints.KitEndpoints.GetKits}/{kitCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<KitResponse>();
        
        var kit = getKitResponse.responseContent!;
        
        var updatedResponse = await
            new HttpRequestBuilder(client, HttpMethod.Put,$"{SettingsEndpoints.KitEndpoints.GetKits}/{kitCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new UpdateKitRequest(
                    ManufactureCode: kit.ManufactureCode,
                    KitSapDefinitionCode: kit.KitSapDefinitionCode,
                    Note: kit.Note,
                    DefiningPackagingCode: kit.DefiningPackagingCode,
                    DryingTime: kit.DryingTime,
                    PackagingQuantities: kit.PackagingQuantities,
                    SpecialInformationSchedules: [
                        new SpecialInformationScheduleContract(specialInformationId, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(3))
                    ]
                ))
                .SendAsync();
        
        return new FullWorkflowApiSeedData(worker1Pin, worker2Pin, kitCode);
    }

    public record FullWorkflowApiSeedData(int Worker1Pin, int Worker2Pin, string KitCodeWithSpecialInformation);
}