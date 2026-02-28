using ErrorOr;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate.Events;

namespace Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;

public class Municipality : AggregateRoot
{
    public string Code { get; private set; }
    public string? Name { get; private set; }
    public string? Status { get; private set; }
    public string? PouCode { get; private set; }
    public string? PouName { get; private set; }
    public string? OrpCode { get; private set; }
    public string? OrpName { get; private set; }
    public string? DistrictCode { get; private set; }
    public string? DistrictName { get; private set; }
    public string? VuscCode { get; private set; }
    public string? VuscName { get; private set; }
    public bool ShouldImportAddressLocation { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Municipality() {}

    private Municipality(
        string code, 
        string? name,
        string? status, 
        string? pouCode, 
        string? pouName, 
        string? orpCode, 
        string? orpName, 
        string? districtCode, 
        string? districtName, 
        string? vuscCode, 
        string? vuscName,
        bool shouldImportAddressLocation,
        DateTimeOffset createdAt, 
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        Name = name;
        Status = status;
        PouCode = pouCode;
        PouName = pouName;
        OrpCode = orpCode;
        OrpName = orpName;
        DistrictCode = districtCode;
        DistrictName = districtName;
        VuscCode = vuscCode;
        VuscName = vuscName;
        ShouldImportAddressLocation = shouldImportAddressLocation;
        CreatedAt = createdAt;
    }
    
    public static ErrorOr<Municipality> CreateFrom(
        string code, 
        string? name,
        string? status, 
        string? pouCode, 
        string? pouName, 
        string? orpCode, 
        string? orpName, 
        string? districtCode, 
        string? districtName, 
        string? vuscCode, 
        string? vuscName,
        bool shouldImportAddressLocation,
        DateTimeOffset createdAt)
    {
        var validation = IsValid(code);

        if (validation.IsError)
            return validation.Errors;
        
        return new Municipality(code, name, status, 
            pouCode, pouName, orpCode, orpName, districtCode, 
            districtName, vuscCode, vuscName, shouldImportAddressLocation, createdAt);
    }
    
    public bool HasSamePropertiesAs(
        string code, 
        string? name,
        string? status, 
        string? pouCode, 
        string? pouName, 
        string? orpCode, 
        string? orpName, 
        string? districtCode, 
        string? districtName, 
        string? vuscCode, 
        string? vuscName)
    {
        if (Code != code.ToUpperInvariant()) return false;
        if (Name != name) return false;
        if (Status != status) return false;
        if (PouCode != pouCode) return false;
        if (PouName != pouName) return false;
        if (OrpCode != orpCode) return false;
        if (OrpName != orpName) return false;
        if (DistrictCode != districtCode) return false;
        if (DistrictName != districtName) return false;
        if (VuscCode != vuscCode) return false;
        if (VuscName != vuscName) return false;
        
        return true;
    }
    
    public ErrorOr<Success> Update(
        string code, 
        string? name,
        string? status, 
        string? pouCode, 
        string? pouName, 
        string? orpCode, 
        string? orpName, 
        string? districtCode, 
        string? districtName, 
        string? vuscCode, 
        string? vuscName,
        bool shouldImportAddressLocation)
    {
        var validation = IsValid(code);

        if (validation.IsError)
            return validation.Errors;
        
        Code = code.ToUpperInvariant();
        Name = name;
        Status = status;
        PouCode = pouCode;
        PouName = pouName;
        OrpCode = orpCode;
        OrpName = orpName;
        DistrictCode = districtCode;
        DistrictName = districtName;
        VuscCode = vuscCode;
        VuscName = vuscName;
        ShouldImportAddressLocation = shouldImportAddressLocation;
        
        return Result.Success;
    }
    
    private static ErrorOr<Success> IsValid(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return MunicipalityDomainErrors.ValidationCodeIsRequired();
        
        return Result.Success;
    }

    public void Apply(MunicipalityCreatedEvent @event)
    {
        Code = @event.Code;
        Name = @event.Name;
        Status = @event.Status;
        PouCode = @event.PouCode;
        PouName = @event.PouName;
        OrpCode = @event.OrpCode;
        OrpName = @event.OrpName;
        DistrictCode = @event.DistrictCode;
        DistrictName = @event.DistrictName;
        VuscCode = @event.VuscCode;
        VuscName = @event.VuscName;
        CreatedAt = @event.CreatedAt;
        ShouldImportAddressLocation = @event.ShouldImportAddressLocation;
    }
    
    public void Apply(MunicipalityUpdatedEvent @event)
    {
        Code = @event.Code;
        Name = @event.Name;
        Status = @event.Status;
        PouCode = @event.PouCode;
        PouName = @event.PouName;
        OrpCode = @event.OrpCode;
        OrpName = @event.OrpName;
        DistrictCode = @event.DistrictCode;
        DistrictName = @event.DistrictName;
        VuscCode = @event.VuscCode;
        VuscName = @event.VuscName;
        ShouldImportAddressLocation = @event.ShouldImportAddressLocation;
    }
}