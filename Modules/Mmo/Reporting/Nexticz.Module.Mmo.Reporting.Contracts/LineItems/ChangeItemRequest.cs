using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Reporting.Contracts.LineItems;

public record ChangeItemRequest(
    [property: Required] ChangeLineItemTypeContract Type,
    [property: Required] DateTimeOffset CutTime,
    Guid? InactivityReasonId);