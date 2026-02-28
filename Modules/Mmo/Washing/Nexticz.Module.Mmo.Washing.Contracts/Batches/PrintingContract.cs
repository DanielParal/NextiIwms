using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Washing.Contracts.Batches;

public record PrintingContract(
    [property: Required] Guid Id,
    [property: Required] Guid BatchId,
    [property: Required] Guid KitId,
    [property: Required] DateTimeOffset DatePrinted,
    [property: Required] PrintingStatusContract Status,
    [property: Required] PrintingTypeContract Type,
    string? FailureReason);