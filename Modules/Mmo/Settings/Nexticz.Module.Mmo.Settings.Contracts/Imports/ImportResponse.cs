using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Imports;

public record ImportResponse(
    [property: Required] Guid Id,
    [property: Required] string UserName,
    [property: Required] ImportStatusContract Status, 
    [property: Required] ImportTypeContract Type, 
    [property: Required] int ImportedCodesCount, 
    [property: Required] ImportError[] Errors,
    [property: Required] DateTimeOffset DateCreated);