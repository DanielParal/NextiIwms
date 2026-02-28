using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Printers;

public record UpdatePrinterRequest(
    [property: Required] string Name, 
    [property: Required] string Ip);