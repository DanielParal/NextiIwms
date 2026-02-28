using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Kits;

public record KitSpecialInformationResponse(
    [property: Required] Guid Id, 
    [property: Required] string Title, 
    [property: Required] string Description, 
    [property: Required] bool HasFile, 
    [property: Required] DateTimeOffset StartDate, 
    [property: Required] DateTimeOffset EndDate);