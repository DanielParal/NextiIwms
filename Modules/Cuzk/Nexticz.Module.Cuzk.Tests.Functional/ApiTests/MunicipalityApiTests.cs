using System.Net;
using Nexticz.Module.Cuzk.Contracts.Municipalities;
using Nexticz.Module.Cuzk.Presentation;
using Shouldly;

namespace Nexticz.Module.Cuzk.Tests.Functional.ApiTests;

[Collection(nameof(CuzkApiCollection))]
public class MunicipalityApiTests(CuzkApiFactoryFixture fixture)
{
    private readonly HttpClient _client = fixture.Factory.CreateClient();

    [Fact]
    public async Task HappyPathTestWithAllCrudEndpoints_ShouldCreateUpdateAndDeleteMunicipality_WhenCallingAllEndpointsOneByOne()
    {
        
        var code = Guid.NewGuid().ToString().ToUpperInvariant();
        var name = Guid.NewGuid().ToString();
        var status = Guid.NewGuid().ToString();
        var pouCode = "POU1";
        var pouName = "POU Name";
        var orpCode = "ORP1";
        var orpName = "ORP Name";
        var districtCode = "D1";
        var districtName = "District Name";
        var vuscCode = "V1";
        var vuscName = "VUSC Name";

        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CuzkEndpoints.MunicipalityEndpoints.CreateMunicipality)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new CreateMunicipalityRequest
                {
                    MunicipalityCode = code,
                    MunicipalityName = name,
                    MunicipalityStatus = status,
                    PouCode = pouCode,
                    PouName = pouName,
                    OrpCode = orpCode, 
                    OrpName = orpName,
                    DistrictCode = districtCode,
                    DistrictName = districtName,
                    VuscCode = vuscCode,
                    VuscName = vuscName,
                    ShouldImportAddressLocation = true
                })
                .SendAndDeserializeAsync<MunicipalityResponse>();

        var createdId = createdResponse.responseContent!.Id;
        var createdCode = createdResponse.responseContent!.MunicipalityCode;
        await VerifyMunicipalityAsync(createdId, code.ToUpperInvariant(), name, status, pouCode, pouName, orpCode, orpName, districtCode, districtName, vuscCode, vuscName, true);
        
        var updateName = Guid.NewGuid().ToString();
        var updateStatus = Guid.NewGuid().ToString();
        var updatePouCode = "POU2";
        var updatePouName = "POU Name 2";
        var updateOrpCode = "ORP2";
        var updateOrpName = "ORP Name 2";
        var updateDistrictCode = "D2";
        var updateDistrictName = "District Name 2";
        var updateVuscCode = "V2";
        var updateVuscName = "VUSC Name 2";

        var updateBody = new UpdateMunicipalityRequest()
        {
            MunicipalityCode = createdCode,
            MunicipalityName = updateName,
            MunicipalityStatus = updateStatus,
            PouCode = updatePouCode,
            PouName = updatePouName,
            OrpCode = updateOrpCode,
            OrpName = updateOrpName,
            DistrictCode = updateDistrictCode,
            DistrictName = updateDistrictName,
            VuscCode = updateVuscCode,
            VuscName = updateVuscName,
            ShouldImportAddressLocation = false
        };
        await new HttpRequestBuilder(_client, HttpMethod.Put,
                $"{CuzkEndpoints.MunicipalityEndpoints.GetMunicipalities}/{createdCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateBody)
                .SendAsync();

        await VerifyMunicipalityAsync(createdId, code, updateBody.MunicipalityName, updateBody.MunicipalityStatus, updateBody.PouCode, updateBody.PouName, updateBody.OrpCode, updateBody.OrpName, updateBody.DistrictCode, updateBody.DistrictName, updateBody.VuscCode, updateBody.VuscName, updateBody.ShouldImportAddressLocation);
        
        await new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{CuzkEndpoints.MunicipalityEndpoints.GetMunicipalities}/{createdCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        await VerifyNotExistAsync(createdCode);
    }

    
    private async Task VerifyMunicipalityAsync(Guid id, string expectedCode, string expectedName, string expectedStatus, string pouCode, string pouName, string orpCode, string orpName, string districtCode, string districtName, string vuscCode, string vuscName, bool shouldImportAddressLocation)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{CuzkEndpoints.MunicipalityEndpoints.GetMunicipalities}/{expectedCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<MunicipalityResponse>();

        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Id.ShouldBe(id);
        getResponse.responseContent.MunicipalityCode.ShouldBe(expectedCode);
        getResponse.responseContent.MunicipalityName.ShouldBe(expectedName);
        getResponse.responseContent.MunicipalityStatus.ShouldBe(expectedStatus);
        getResponse.responseContent.PouCode.ShouldBe(pouCode);
        getResponse.responseContent.PouName.ShouldBe(pouName);
        getResponse.responseContent.OrpCode.ShouldBe(orpCode); 
        getResponse.responseContent.OrpName.ShouldBe(orpName);
        getResponse.responseContent.DistrictCode.ShouldBe(districtCode);
        getResponse.responseContent.DistrictName.ShouldBe(districtName);
        getResponse.responseContent.VuscCode.ShouldBe(vuscCode);
        getResponse.responseContent.VuscName.ShouldBe(vuscName);
        getResponse.responseContent.ShouldImportAddressLocation.ShouldBe(shouldImportAddressLocation);
    }
    
    private async Task VerifyNotExistAsync(string code)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{CuzkEndpoints.MunicipalityEndpoints.GetMunicipalities}/{code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<MunicipalityResponse>(ensureSuccessStatusCode: false);
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
}