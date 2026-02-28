using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Washing.Contracts.Batches;

public record BatchResponse(
    [property: Required] Guid Id,
    Guid? SisterBatchId,
    [property: Required] string KitCode,
    [property: Required] string PackagingCode,
    [property: Required] string DefiningPackagingCode,
    [property: Required] TimeSpan OptimalKitDuration,
    [property: Required] int FinishedKits,
    [property: Required] int KitsCount,
    [property: Required] string Efficiency,
    [property: Required] KitWashCycleContract[] KitWashCycles,
    [property: Required] PrintingContract[] Printings,
    [property: Required] bool IsSpecialInformationConfirmationNeeded,
    [property: Required] SpecialInformationContract? SpecialInformation,
    [property: Required] bool ShouldBeLogout,
    string? ReasonForLogout,
    [property: Required] bool CanCompleteKit,
    string? ReasonWhyKitCannotBeCompleted,
    [property: Required] string WashingMachineCode,
    [property: Required] bool IsHelpNeeded);