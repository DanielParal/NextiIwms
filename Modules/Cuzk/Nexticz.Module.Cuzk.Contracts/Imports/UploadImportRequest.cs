using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Cuzk.Contracts.Imports;

public record UploadImportRequest([property: Required] ImportTypeContract ImportType, string? CsvDelimiter);