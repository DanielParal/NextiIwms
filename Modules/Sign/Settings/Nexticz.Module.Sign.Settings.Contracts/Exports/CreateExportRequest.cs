using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Exports;

public record CreateExportRequest([property: Required] ExportTypeContract ExportType);