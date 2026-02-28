using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Washing.Contracts.Batches;

public record KitWashCycleContract(
    [property: Required] Guid Id,
    [property: Required] DateTimeOffset StartDate,
    [property: Required] double Efficiency,
    [property: Required] DateTimeOffset EndDate,
    [property: Required] string WorkerName,
    [property: Required] int GlobalKitsCount);