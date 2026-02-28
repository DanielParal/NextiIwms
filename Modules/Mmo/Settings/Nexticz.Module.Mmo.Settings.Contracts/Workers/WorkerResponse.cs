using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Workers;

public record WorkerResponse(
    [property: Required] Guid Id,
    [property: Required] string Name, 
    [property: Required] int Pin, 
    [property: Required] bool IsActive);