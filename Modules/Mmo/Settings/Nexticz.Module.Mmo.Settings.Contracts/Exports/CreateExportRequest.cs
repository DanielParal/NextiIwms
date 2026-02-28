using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Exports;

public record CreateExportRequest([property: Required] ExportTypeContract ExportType);