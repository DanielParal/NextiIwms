using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Exports;

public record ExportResponse(
    [property: Required] Guid Id,
    [property: Required] string UserName,
    [property: Required] ExportTypeContract Type,
    [property: Required] DateTimeOffset DateCreated);