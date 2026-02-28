using Nexticz.Module.Mmo.Settings.Contracts.Depositors;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Depositors;

internal class DepositorResponseFactory
{
    public static DepositorResponse Create(Depositor depositor)
    {
        return new DepositorResponse(depositor.Id, depositor.Code, depositor.Name, depositor.BarcodeTemplate);
    }
}