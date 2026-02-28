
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;
using Shouldly;

namespace Nexticz.Module.Cuzk.Tests.Unit.Domain;

public class AddressLocationTests
{
    [Fact]
    public void Update_WithValidData_ShouldSucceed()
    {
        var addressLocation = AddressLocation.CreateFrom(
            "123",
            "munCode",
            "munName",
            "distCode",
            "distName",
            "momcCode",
            "momcName",
            "pragueCode",
            "pragueName",
            "streetCode",
            "streetName",
            "districtCode",
            "districtName",
            "countryCode",
            "countryName",
            "soType",
            "numDesc",
            "numRef",
            "numRefChar",
            "zipCode",
            "1.0m",
            "2.0m",
            "2.0m",
            "2.0m",
            "2.0m",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);
        
        var result = addressLocation.Value.Update(
            "123",
            "newMunCode",
            "newMunName",
            "newDistCode",
            "newDistName",
            "newMomcCode",
            "newMomcName",
            "newPragueCode",
            "newPragueName",
            "newStreetCode",
            "newStreetName",
            "newDistrictCode",
            "newDistrictName",
            "newCountryCode",
            "newCountryName",
            "newSoType",
            "newNumDesc",
            "newNumRef",
            "newNumRefChar",
            "newZipCode",
            "1.0m",
            "2.0m",
            "2.0m",
            "2.0m",
            "2.0m",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);
        
        result.IsError.ShouldBeFalse();
        addressLocation.Value.MunicipalityCode.ShouldBe("newMunCode");
        addressLocation.Value.MunicipalityName.ShouldBe("newMunName");
    }

    [Fact]
    public void Create_WithEmptyAdmCode_ShouldFail()
    {
        var addressLocation = AddressLocation.CreateFrom(
            "",
            "munCode",
            "munName",
            "distCode",
            "distName",
            "momcCode",
            "momcName",
            "pragueCode",
            "pragueName",
            "streetCode",
            "streetName",
            "districtCode",
            "districtName",
            "countryCode",
            "countryName",
            "soType",
            "numDesc",
            "numRef",
            "numRefChar",
            "zipCode",
            "1.0m",
            "2.0m",
            "2.0m",
            "2.0m",
            "2.0m",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);
        
        addressLocation.IsError.ShouldBeTrue();
        addressLocation.Errors.Count.ShouldBe(1);
        addressLocation.Errors.First().Code.ShouldBe(AddressLocationDomainErrors.ValidationAdmCodeIsRequired.Code);
    }
    
    [Fact]
    public void Create_WithEmptyMunicipalityCode_ShouldFail()
    {
        var addressLocation = AddressLocation.CreateFrom(
            "AdmCode",
            "",
            "munName",
            "distCode",
            "distName",
            "momcCode",
            "momcName",
            "pragueCode",
            "pragueName",
            "streetCode",
            "streetName",
            "districtCode",
            "districtName",
            "countryCode",
            "countryName",
            "soType",
            "numDesc",
            "numRef",
            "numRefChar",
            "zipCode",
            "1.0m",
            "2.0m",
            "2.0m",
            "2.0m",
            "2.0m",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);
        
        addressLocation.IsError.ShouldBeTrue();
        addressLocation.Errors.Count.ShouldBe(1);
        addressLocation.Errors.First().Code.ShouldBe(AddressLocationDomainErrors.ValidationMunicipalityCodeIsRequired.Code);
    }
}