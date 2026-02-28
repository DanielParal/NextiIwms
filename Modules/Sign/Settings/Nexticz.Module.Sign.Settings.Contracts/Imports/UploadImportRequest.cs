using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Imports;

public record UploadImportRequest([property: Required] ImportTypeContract ImportType);