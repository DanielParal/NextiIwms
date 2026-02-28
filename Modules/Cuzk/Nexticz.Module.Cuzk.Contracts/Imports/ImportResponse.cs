using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Cuzk.Contracts.Imports;

public record ImportResponse(
    [property: Required] Guid Id,
    [property: Required] string UserName,
    [property: Required] string FileName,
    [property: Required] ImportTypeContract Type,
    [property: Required] ImportStatusContract Status,
    [property: Required] string[] ImportedCodes,
    [property: Required] string[] ImportErrorMessages,
    [property: Required] DateTimeOffset DateRequested,
    [property: Required] DateTimeOffset? DateImported
    );