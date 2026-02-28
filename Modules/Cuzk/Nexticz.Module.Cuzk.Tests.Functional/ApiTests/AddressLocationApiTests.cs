using System.Net;
using Nexticz.Module.Cuzk.Contracts.AddressLocations;
using Nexticz.Module.Cuzk.Presentation;
using Shouldly;

namespace Nexticz.Module.Cuzk.Tests.Functional.ApiTests;

[Collection(nameof(CuzkApiCollection))]
public class AddressLocationApiTests(CuzkApiFactoryFixture fixture)
{
    private readonly HttpClient _client = fixture.Factory.CreateClient();

    [Fact]
    public async Task HappyPathTestWithAllCrudEndpoints_ShouldCreateUpdateAndDeleteAddressLocation_WhenCallingAllEndpointsOneByOne()
    {
        var admCode = Guid.NewGuid().ToString().ToUpperInvariant();
        var municipalityCode = Guid.NewGuid().ToString();
        var municipalityName = "Test Address Location";
        var municipalityDistrictCode = "MDC1";
        var municipalityDistrictName = "MD Name";
        var momcCode = "MOMC1";
        var momcName = "MOMC Name";
        var pragueDistrictCode = "PD1";
        var pragueDistrictName = "Prague District";
        var streetCode = "STR1";
        var streetName = "Street Name";
        var districtCode = "D1";
        var districtName = "District Name";
        var countryCode = "CZ";
        var countryName = "Czech Republic";
        var soType = "Type1";
        var numberDescriptive = "123";
        var numberReference = "456";
        var numberReferenceChar = "A";
        var zipCode = "12345";
        var krovakX = "1234.56";
        var krovakY = "7890.12";
        var latitude = "50.123456";
        var longitude = "14.123456";
        var altitude = "200";
        var slug = "test-slug";
        var slugNormalized = "test-slug-normalized";
        var slugStreet = "test-street";
        var validFrom = DateTime.UtcNow;

        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, CuzkEndpoints.AddressLocationEndpoints.CreateAddressLocation)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new CreateAddressLocationRequest()
                {
                    AdmCode = admCode,
                    MunicipalityCode = municipalityCode,
                    MunicipalityName = municipalityName,
                    MunicipalityDistrictCode = municipalityDistrictCode,
                    MunicipalityDistrictName = municipalityDistrictName,
                    MomcCode = momcCode,
                    MomcName = momcName,
                    PragueDistrictCode = pragueDistrictCode,
                    PragueDistrictName = pragueDistrictName,
                    StreetCode = streetCode,
                    StreetName = streetName,
                    DistrictCode = districtCode,
                    DistrictName = districtName,
                    CountryCode = countryCode,
                    CountryName = countryName,
                    SoType = soType,
                    NumberDescriptive = numberDescriptive,
                    NumberReference = numberReference,
                    NumberReferenceChar = numberReferenceChar,
                    ZipCode = zipCode,
                    KrovakX = krovakX,
                    KrovakY = krovakY,
                    Latitude = latitude,
                    Longitude = longitude,
                    Altitude = altitude,
                    ValidFrom = validFrom
                })
                .SendAndDeserializeAsync<AddressLocationResponse>();

        var createdId = createdResponse.responseContent!.Id;
        var createdCode = createdResponse.responseContent!.AdmCode;
        await VerifyAddressLocationAsync(createdId, admCode, municipalityCode, municipalityName);

        var updateBody = new UpdateAddressLocationRequest()
        {
            MunicipalityCode = municipalityCode,
            MunicipalityName = "Updated Address Location Name",
            MunicipalityDistrictCode = "UPD-MDC1",
            MunicipalityDistrictName = "Updated MD Name",
            MomcCode = "UPD-MOMC1",
            MomcName = "Updated MOMC Name",
            PragueDistrictCode = "UPD-PD1",
            PragueDistrictName = "Updated Prague District",
            StreetCode = "UPD-STR1",
            StreetName = "Updated Street Name",
            DistrictCode = "UPD-D1",
            DistrictName = "Updated District Name",
            CountryCode = "SK",
            CountryName = "Slovakia",
            SoType = "UpdatedType1",
            NumberDescriptive = "789",
            NumberReference = "012",
            NumberReferenceChar = "B",
            ZipCode = "54321",
            KrovakX = "9876.54",
            KrovakY = "5432.10",
            Latitude = "49.987654",
            Longitude = "15.987654",
            Altitude = "300",
            ValidFrom = DateTime.UtcNow.AddDays(1)
        };
        
        await new HttpRequestBuilder(_client, HttpMethod.Put,
                $"{CuzkEndpoints.AddressLocationEndpoints.GetAddressLocations}/{createdCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateBody)
                .SendAsync();

        await VerifyAddressLocationAsync(createdId, admCode, updateBody.MunicipalityCode, updateBody.MunicipalityName);
        
        await new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{CuzkEndpoints.AddressLocationEndpoints.GetAddressLocations}/{createdCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        await VerifyNotExistAsync(createdCode);
    }

    
    private async Task VerifyAddressLocationAsync(Guid id, string expectedCode, string expectedMunicipalityCode, string expectedMunicipalityName)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{CuzkEndpoints.AddressLocationEndpoints.GetAddressLocations}/{expectedCode}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<AddressLocationResponse>();

        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Id.ShouldBe(id);
        getResponse.responseContent.AdmCode.ShouldBe(expectedCode);
        getResponse.responseContent.MunicipalityCode.ShouldBe(expectedMunicipalityCode);
        getResponse.responseContent.MunicipalityName.ShouldBe(expectedMunicipalityName);
    }
    
    private async Task VerifyNotExistAsync(string code)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{CuzkEndpoints.AddressLocationEndpoints.GetAddressLocations}/{code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<AddressLocationResponse>(ensureSuccessStatusCode: false);
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
}