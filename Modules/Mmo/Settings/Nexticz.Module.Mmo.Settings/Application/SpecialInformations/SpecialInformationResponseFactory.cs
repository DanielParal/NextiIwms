using Nexticz.Module.Mmo.Settings.Contracts.SpecialInformations;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations;

internal static class SpecialInformationResponseFactory
{
    public static SpecialInformationResponse Create(SpecialInformation specialInformation)
    {
        return new SpecialInformationResponse(
            specialInformation.Id, specialInformation.Title, 
            specialInformation.Description, specialInformation.HasFile);
    }
}