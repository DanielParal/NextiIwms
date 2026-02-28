using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Workers;

public record CreateWorkerRequest(
    [property: Required] string Name, 
    [property: Required] int Pin, 
    [property: Required] bool IsActive);