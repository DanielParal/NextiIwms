using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Printers;

public record CreatePrinterRequest(
    [property: Required] string Code, 
    [property: Required] string Name, 
    [property: Required] string Ip);