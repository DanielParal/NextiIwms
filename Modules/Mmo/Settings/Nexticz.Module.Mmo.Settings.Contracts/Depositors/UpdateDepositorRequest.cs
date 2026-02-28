using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Depositors;

public record UpdateDepositorRequest(
    [property: Required] string Name,
    string? BarcodeTemplate);