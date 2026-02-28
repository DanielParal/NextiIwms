using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Packagings;

public record DimensionsContract(
    [property: Required] decimal Depth, 
    [property: Required] decimal Width, 
    [property: Required] decimal Height);