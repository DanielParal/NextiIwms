using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Kits;

public record SpecialInformationScheduleContract(
    [property: Required] Guid Id, 
    [property: Required] DateTimeOffset StartDate, 
    [property: Required] DateTimeOffset EndDate);