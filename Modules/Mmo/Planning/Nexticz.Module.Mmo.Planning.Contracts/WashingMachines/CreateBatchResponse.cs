using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

public record CreateBatchResponse(
    [property: Required] BatchContract Batch,
    BatchContract? SisterBatch
    );