using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.DepositorGroups;

public record UpdateDepositorGroupRequest([property: Required] string Name);