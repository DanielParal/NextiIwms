using Nexticz.Module.Sign.Settings.Contracts.DepositorGroups;
using Nexticz.Module.Sign.Settings.Contracts.Depositors;
using Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;
using Nexticz.Module.Sign.Settings.Contracts.Locations;
using Nexticz.Module.Sign.Settings.Contracts.Partners;
using Nexticz.Module.Sign.Settings.Contracts.Printers;
using Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate;
using Nexticz.Module.Sign.Settings.Presentation;
using Nexticz.Module.Sign.SharedTesting;

namespace Nexticz.Module.Sign.Settings.Tests.Functional;

public class SettingsDbSeeder
{
    public static async Task<ApiSeedData> SeedDataAsync(HttpClient client)
    {
        var location1CreatedResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post, SettingsEndpoints.LocationEndpoints.CreateLocation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = $"Location_{Guid.NewGuid()}",
                    Name = Guid.NewGuid()
                })
                .SendAndDeserializeAsync<LocationResponse>();
        
        var location2CreatedResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post, SettingsEndpoints.LocationEndpoints.CreateLocation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = $"Location_{Guid.NewGuid()}",
                    Name = Guid.NewGuid()
                })
                .SendAndDeserializeAsync<LocationResponse>();
        
        var printer1CreatedResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post, SettingsEndpoints.PrinterEndpoints.CreatePrinter)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = $"Printer_{Guid.NewGuid()}",
                    Name = Guid.NewGuid(),
                    Ip = Guid.NewGuid(),
                })
                .SendAndDeserializeAsync<PrinterResponse>();
        
        var printer2CreatedResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post, SettingsEndpoints.PrinterEndpoints.CreatePrinter)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = $"Printer_{Guid.NewGuid()}",
                    Name = Guid.NewGuid(),
                    Ip = Guid.NewGuid(),
                })
                .SendAndDeserializeAsync<PrinterResponse>();
        
        var depositorGroupCreatedResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post, SettingsEndpoints.DepositorGroupEndpoints.CreateDepositorGroup)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = $"DepositorGroup_{Guid.NewGuid()}",
                    Name = Guid.NewGuid()
                })
                .SendAndDeserializeAsync<DepositorGroupResponse>();
        
        var deliveryDocumentTemplateCreated = await
            new HttpRequestBuilder(client, HttpMethod.Post, SettingsEndpoints.DocumentTemplateEndpoints.CreateDocumentTemplate)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = Guid.NewGuid(),
                    TextOffsets = new List<TextOffsetContract>(),
                    TextBackgrounds = new List<TextBackgroundContract>()
                })
                .SendAndDeserializeAsync<DocumentTemplateResponse>();
        
        var loadingDocumentTemplateCreated = await
            new HttpRequestBuilder(client, HttpMethod.Post, SettingsEndpoints.DocumentTemplateEndpoints.CreateDocumentTemplate)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = Guid.NewGuid(),
                    TextOffsets = new List<TextOffsetContract>(),
                    TextBackgrounds = new List<TextBackgroundContract>()
                })
                .SendAndDeserializeAsync<DocumentTemplateResponse>();
        
        var depositorCreatedResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post, SettingsEndpoints.DepositorEndpoints.CreateDepositor)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = Guid.NewGuid(),
                    Name = Guid.NewGuid(),
                    DepositorGroupCode = depositorGroupCreatedResponse.responseContent!.Code,
                    DeliveryTemplateCode = deliveryDocumentTemplateCreated.responseContent!.Code,
                    LoadingTemplateCode = loadingDocumentTemplateCreated.responseContent!.Code,
                })
                .SendAndDeserializeAsync<DepositorResponse>();
        
        var partnerCreatedResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post, SettingsEndpoints.PartnerEndpoints.CreatePartner)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = $"Partner_{Guid.NewGuid()}",
                    Name = Guid.NewGuid()
                })
                .SendAndDeserializeAsync<PartnerResponse>();
        
        var signingDeviceCreatedResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post, SettingsEndpoints.SigningDeviceEndpoints.CreateSigningDevice)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = $"Tablet_{Guid.NewGuid()}",
                    Name = Guid.NewGuid(),
                    IsActive = true,
                    LocationCode = location1CreatedResponse.responseContent!.Code,
                    PrinterCode = printer1CreatedResponse.responseContent!.Code
                })
                .SendAndDeserializeAsync<PartnerResponse>();
        
        return new ApiSeedData(
            location1CreatedResponse.responseContent!.Code, 
            location2CreatedResponse.responseContent!.Code,
            printer1CreatedResponse.responseContent!.Code,
            printer2CreatedResponse.responseContent!.Code,
            depositorGroupCreatedResponse.responseContent!.Code,
            partnerCreatedResponse.responseContent!.Code,
            signingDeviceCreatedResponse.responseContent!.Code,
            depositorCreatedResponse.responseContent!.Code,
            deliveryDocumentTemplateCreated.responseContent!.Code,
            loadingDocumentTemplateCreated.responseContent!.Code);
    }

    public record ApiSeedData(
        string LocationCode1, 
        string LocationCode2,
        string PrinterCode1,
        string PrinterCode2,
        string DepositorGroupCode,
        string PartnerCode,
        string SigningDeviceCode,
        string DepositorCode,
        string DeliveryDocumentTemplateCode,
        string LoadingDocumentTemplateCode);
}