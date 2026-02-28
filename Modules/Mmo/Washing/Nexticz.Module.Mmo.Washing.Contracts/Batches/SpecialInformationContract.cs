using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Washing.Contracts.Batches;

public record SpecialInformationContract(
    [property: Required] Guid Id,
    [property: Required] string Title,
    [property: Required] string Description,
    [property: Required] bool HasFile
    );