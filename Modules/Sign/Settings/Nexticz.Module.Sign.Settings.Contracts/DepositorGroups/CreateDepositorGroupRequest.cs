using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.DepositorGroups;

public record CreateDepositorGroupRequest([property: Required] string Code, [property: Required] string Name);