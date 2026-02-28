using Nexticz.Module.Mmo.Settings.Contracts.Depositors;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Module.Mmo.Settings.Contracts.KitTypes;
using Nexticz.Module.Mmo.Settings.Contracts.Manufactures;
using Nexticz.Module.Mmo.Settings.Contracts.PackagingCirculations;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Module.Mmo.Settings.Contracts.PackagingTypes;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Settings.Presentation;
using Nexticz.Module.Mmo.Settings.Tests.Functional.ApiTests;
using Nexticz.Module.Mmo.SharedTesting;

namespace Nexticz.Module.Mmo.Settings.Tests.Functional;

public class SettingsDbSeeder
{
    public static async Task<ApiSeedData> SeedDataAsync(HttpClient client)
    {
        var createdDepositorResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post,SettingsEndpoints.DepositorEndpoints.CreateDepositor)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "D001",
                    Name = "Depositor 1",
                    BarcodeTemplate = "*{{KitNumber}}401KOMPLETY*"
                })
                .SendAndDeserializeAsync<DepositorResponse>();
        
        var depositorCode = createdDepositorResponse.responseContent!.Code;
        var depositorName = createdDepositorResponse.responseContent!.Name;
    
        var createdPackagingTypeResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post,SettingsEndpoints.PackagingTypeEndpoints.CreatePackagingType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "PT001",
                    Name = "Packaging type 1"
                })
                .SendAndDeserializeAsync<PackagingTypeResponse>();

        var packagingTypeCode = createdPackagingTypeResponse.responseContent!.Code;
        var packagingTypeName = createdPackagingTypeResponse.responseContent!.Name;
        
        var createdPackagingCirculationResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post,SettingsEndpoints.PackagingCirculationEndpoints.CreatePackagingCirculation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "PC001",
                    Name = "Packaging circulation 1"
                })
                .SendAndDeserializeAsync<PackagingCirculationResponse>();

        var packagingCirculationCode = createdPackagingCirculationResponse.responseContent!.Code;
        
        var createdKitTypeResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post,SettingsEndpoints.KitTypeEndpoints.CreateKitType)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "KT001",
                    Name = "Kit type 1"
                })
                .SendAndDeserializeAsync<KitTypeResponse>();

        var kitTypeCode = createdKitTypeResponse.responseContent!.Code;
        
        var createdKitSapDefinitionResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post,SettingsEndpoints.KitSapDefinitionEndpoints.CreateKitSapDefinition)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "KIT_04",
                    Name = "Kit 04"
                })
                .SendAndDeserializeAsync<KitTypeResponse>();

        var kitSapDefinitionCode = createdKitSapDefinitionResponse.responseContent!.Code;
        
        var createdManufactureResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post,SettingsEndpoints.ManufactureEndpoints.CreateManufacture)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = "MAN001",
                    Name = "Manufacture 1"
                })
                .SendAndDeserializeAsync<ManufactureResponse>();

        var manufactureCode = createdManufactureResponse.responseContent!.Code;
        
        var washingMachineResponse1 = await
            new HttpRequestBuilder(client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(WashingMachineApiTests.GenerateCreateRequest(code: "MYCKA_1"))
                .SendAndDeserializeAsync<WashingMachineResponse>();

        var washingMachine1 = washingMachineResponse1.responseContent!.Code;
        
        var washingMachineResponse2 = await
            new HttpRequestBuilder(client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(WashingMachineApiTests.GenerateCreateRequest(code: "MYCKA_2"))
                .SendAndDeserializeAsync<WashingMachineResponse>();

        var washingMachine2 = washingMachineResponse2.responseContent!.Code;
        
        var washingMachineResponse3 = await
            new HttpRequestBuilder(client, HttpMethod.Post,SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(WashingMachineApiTests.GenerateCreateRequest(code: "MYCKA_3"))
                .SendAndDeserializeAsync<WashingMachineResponse>();

        var washingMachine3 = washingMachineResponse3.responseContent!.Code;
        
        const string packagingCustomerNumber = "185432967";
        var packagingResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post,SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new CreatePackagingRequest(
                    packagingTypeCode,
                    depositorCode,
                    packagingCirculationCode,
                    packagingCustomerNumber,
                    "Packaging 1",
                    true,
                    new DimensionsContract(10, 20, 30),
                    5,
                    [
                        new WashingMachineSpeedContract(washingMachine1, WashingMachineSpeedLevelContract.Speed1),
                        new WashingMachineSpeedContract(washingMachine2, WashingMachineSpeedLevelContract.Speed2),
                        new WashingMachineSpeedContract(washingMachine3, WashingMachineSpeedLevelContract.NotSet)
                    ]))
                .SendAndDeserializeAsync<PackagingResponse>();
        
        var packagingCode = packagingResponse.responseContent!.Code;
        
        var packagingResponse2 = await
            new HttpRequestBuilder(client, HttpMethod.Post,SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new CreatePackagingRequest(
                    packagingTypeCode,
                    depositorCode,
                    packagingCirculationCode,
                    "26854875",
                    "Packaging 2",
                    true,
                    new DimensionsContract(21, 32, 43),
                    15,
                    [
                        new WashingMachineSpeedContract(washingMachine1, WashingMachineSpeedLevelContract.Speed2),
                        new WashingMachineSpeedContract(washingMachine2, WashingMachineSpeedLevelContract.Speed1),
                        new WashingMachineSpeedContract(washingMachine3, WashingMachineSpeedLevelContract.Speed1)
                    ]))
                .SendAndDeserializeAsync<PackagingResponse>();
        
        var packagingCode2 = packagingResponse2.responseContent!.Code;
        
        var packagingResponse3 = await
            new HttpRequestBuilder(client, HttpMethod.Post,SettingsEndpoints.PackagingEndpoints.CreatePackaging)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new CreatePackagingRequest(
                    packagingTypeCode,
                    depositorCode,
                    packagingCirculationCode,
                    "3856954785",
                    "Packaging 3",
                    true,
                    new DimensionsContract(31, 12, 62),
                    25,
                    [
                        new WashingMachineSpeedContract(washingMachine1, WashingMachineSpeedLevelContract.Speed1),
                        new WashingMachineSpeedContract(washingMachine2, WashingMachineSpeedLevelContract.NotSet),
                        new WashingMachineSpeedContract(washingMachine3, WashingMachineSpeedLevelContract.NotSet)
                    ]))
                .SendAndDeserializeAsync<PackagingResponse>();
        
        var packagingCode3 = packagingResponse3.responseContent!.Code;
        
        var kitDefiningPackagingNumber = depositorCode + packagingCustomerNumber;
        var kitResponse = await
            new HttpRequestBuilder(client, HttpMethod.Post,SettingsEndpoints.KitEndpoints.CreateKit)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new CreateKitRequest(
                    kitTypeCode,
                    kitSapDefinitionCode,
                    depositorCode,
                    manufactureCode,
                    "985432167",
                    "Kit 1 Note",
                    kitDefiningPackagingNumber,
                    120,
                    [
                        new PackagingQuantityContract(packagingCode, string.Empty, 3),
                        new PackagingQuantityContract(packagingCode2, string.Empty, 1),
                        new PackagingQuantityContract(packagingCode3, string.Empty, 20)
                    ],
                    []))
                .SendAndDeserializeAsync<KitResponse>();
        
        var kitCode = kitResponse.responseContent!.Code;
        
        return new ApiSeedData(depositorCode, depositorName, 
            packagingTypeCode, packagingTypeName, packagingCirculationCode, kitTypeCode, kitSapDefinitionCode, manufactureCode, 
            kitCode, kitDefiningPackagingNumber, packagingCode, packagingCode2, packagingCode3,
            washingMachine1, washingMachine2, washingMachine3);
    }

    public class ApiSeedData
    {
        public string DepositorCode { get; private set; }
        public string DepositorName { get; private set; }
        public string PackagingTypeCode { get; private set; }
        public string PackagingTypeName { get; private set; }
        public string PackagingCirculationCode { get; private set; }
        public string KitTypeCode { get; private set; }
        public string KitSapDefinitionCode { get; private set; }
        public string ManufactureCode { get; private set; }
        public string KitCode { get; private set; }
        public string KitDefiningPackagingNumber { get; private set; }
        public string PackagingCode { get; private set; }
        public string PackagingCode2 { get; private set; }
        public string PackagingCode3 { get; private set; }
        public string WashingMachineCode1 { get; private set; }
        public string WashingMachineCode2 { get; private set; }
        public string WashingMachineCode3 { get; private set; }

        public ApiSeedData(
            string depositorCode, 
            string depositorName,
            string packagingTypeCode, 
            string packagingTypeName, 
            string packagingCirculationCode, 
            string kitTypeCode, 
            string kitSapDefinitionCode, 
            string manufactureCode,
            string kitCode,
            string kitDefiningPackagingNumber,
            string packagingCode,
            string packagingCode2,
            string packagingCode3,
            string washingMachineCode1,
            string washingMachineCode2,
            string washingMachineCode3)
        {
            DepositorCode = depositorCode;
            DepositorName = depositorName;
            PackagingTypeCode = packagingTypeCode;
            PackagingTypeName = packagingTypeName;
            PackagingCirculationCode = packagingCirculationCode;
            KitTypeCode = kitTypeCode;
            KitSapDefinitionCode = kitSapDefinitionCode;
            ManufactureCode = manufactureCode;
            KitCode = kitCode;
            KitDefiningPackagingNumber = kitDefiningPackagingNumber;
            PackagingCode = packagingCode;
            PackagingCode2 = packagingCode2;
            PackagingCode3 = packagingCode3;
            WashingMachineCode1 = washingMachineCode1;
            WashingMachineCode2 = washingMachineCode2;
            WashingMachineCode3 = washingMachineCode3;
        }
    }
}