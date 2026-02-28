using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Depositors;

public record UpdateDepositorRequest(
    [property: Required] string Name, 
    [property: Required] string DepositorGroupCode,
    [property: Required] string DeliveryTemplateCode,
    [property: Required] string LoadingTemplateCode);