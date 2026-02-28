using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Depositors;

public record DepositorResponse(
    [property: Required] Guid Id, 
    [property: Required] string Code, 
    [property: Required] string Name, 
    [property: Required] string DepositorGroupCode,
    [property: Required] string DeliveryTemplateCode,
    [property: Required] string LoadingTemplateCode);