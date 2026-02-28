using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

public record CreateBatchRequest(
    [property: Required] string LineQueueCode, 
    [property: Required] string KitCode, 
    [property: Required] int KitsCount, 
    [property: Required] string PackagingCode, 
    string? SisterPackagingCode);