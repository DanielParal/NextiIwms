using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Reporting.Contracts.LineItems;

public record AddItemsRequest(
    [property: Required] Guid ShiftId, 
    [property: Required] DateTimeOffset StartDate,
    [property: Required] DateTimeOffset EndDate,
    [property: Required] AddLineItemTypeContract Type,
    [property: Required] string[] LineCodes);