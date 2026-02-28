
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;
using Shouldly;

namespace Nexticz.Module.Cuzk.Tests.Unit.Domain;

public class MunicipalityTests
{
    private readonly string _code = "123";
    private readonly string _name = "Test Municipality";
    private readonly string _status = "Active";
    private readonly string _pouCode = "POU123";
    private readonly string _pouName = "Test POU";
    private readonly string _orpCode = "ORP123";
    private readonly string _orpName = "Test ORP";
    private readonly string _districtCode = "CZ0123";
    private readonly string _districtName = "Test District";
    private readonly string _vuscCode = "VUSC123";
    private readonly string _vuscName = "Test VUSC";
    private readonly bool _shouldImportAddressLocation = false;
    private readonly DateTimeOffset _createdAt = DateTimeOffset.UtcNow;

    [Fact]
    public void Create_WithValidData_ShouldCreateMunicipality()
    {
        // Act
        var municipality = Municipality.CreateFrom(
            _code,
            _name,
            _status,
            _pouCode,
            _pouName,
            _orpCode,
            _orpName,
            _districtCode,
            _districtName,
            _vuscCode,
            _vuscName,
            _shouldImportAddressLocation,
            _createdAt);

        // Assert
        municipality.IsError.ShouldBeFalse();
        municipality.Value.Code.ShouldBe(_code);
        municipality.Value.Name.ShouldBe(_name);
        municipality.Value.Status.ShouldBe(_status);
        municipality.Value.PouCode.ShouldBe(_pouCode);
        municipality.Value.PouName.ShouldBe(_pouName);
        municipality.Value.OrpCode.ShouldBe(_orpCode);
        municipality.Value.OrpName.ShouldBe(_orpName);
        municipality.Value.DistrictCode.ShouldBe(_districtCode);
        municipality.Value.DistrictName.ShouldBe(_districtName);
        municipality.Value.VuscCode.ShouldBe(_vuscCode);
        municipality.Value.VuscName.ShouldBe(_vuscName);
        municipality.Value.CreatedAt.ShouldBe(_createdAt);
    }
    
    [Fact]
    public void Create_WithEmptycode_ShouldCreateMunicipality()
    {
        // Act
        var municipality = Municipality.CreateFrom(
            "",
            _name,
            _status,
            _pouCode,
            _pouName,
            _orpCode,
            _orpName,
            _districtCode,
            _districtName,
            _vuscCode,
            _vuscName,
            _shouldImportAddressLocation,
            _createdAt);

        // Assert
        municipality.IsError.ShouldBeTrue();
        municipality.Errors.Count.ShouldBe(1);
        municipality.Errors.First().Code.ShouldBe(MunicipalityDomainErrors.ValidationCodeIsRequired().Code);
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateMunicipality()
    {
        // Arrange
        var municipality = Municipality.CreateFrom(
            _code,
            _name,
            _status,
            _pouCode,
            _pouName,
            _orpCode,
            _orpName,
            _districtCode,
            _districtName,
            _vuscCode,
            _vuscName,
            _shouldImportAddressLocation,
            _createdAt);

        var newName = "Updated Municipality";
        var newStatus = "Inactive";

        // Act
        var updateResult = municipality.Value.Update(
            _code,
            newName,
            newStatus,
            _pouCode,
            _pouName,
            _orpCode,
            _orpName,
            _districtCode,
            _districtName,
            _vuscCode,
            _vuscName,
            _shouldImportAddressLocation);

        // Assert
        updateResult.IsError.ShouldBeFalse();
        municipality.Value.Name.ShouldBe(newName);
        municipality.Value.Status.ShouldBe(newStatus);
    }
}