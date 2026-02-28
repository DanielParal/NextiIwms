using Nexticz.Module.Sign.Settings.Contracts.Depositors;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Depositors;

internal static class DepositorResponseFactory
{
    public static DepositorResponse Create(Depositor depositor)
    {
        return new DepositorResponse(
            depositor.Id,
            depositor.Code, 
            depositor.Name, 
            depositor.DepositorGroupCode,
            depositor.DeliveryTemplateCode,
            depositor.LoadingTemplateCode);
    }
}