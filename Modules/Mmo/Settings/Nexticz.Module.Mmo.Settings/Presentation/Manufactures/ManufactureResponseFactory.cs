using Nexticz.Module.Mmo.Settings.Contracts.Manufactures;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;

namespace Nexticz.Module.Mmo.Settings.Presentation.Manufactures;

internal class ManufactureResponseFactory
{
    public static ManufactureResponse Create(Manufacture manufacture)
    {
        return new ManufactureResponse(manufacture.Id, manufacture.Code, manufacture.Name);
    }
}