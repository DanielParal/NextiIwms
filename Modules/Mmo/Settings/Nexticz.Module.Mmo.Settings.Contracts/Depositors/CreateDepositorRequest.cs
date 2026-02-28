using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Depositors;

public record CreateDepositorRequest(
    [property: Required] string Code, 
    [property: Required] string Name,
    string? BarcodeTemplate);