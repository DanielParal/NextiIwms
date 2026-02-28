using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Kits;

internal static class KitSpecialInformationResponseFactory
{
    public static KitSpecialInformationResponse Create(SpecialInformationSchedule schedule, SpecialInformation? specialInformation)
    {
        return new KitSpecialInformationResponse(
            schedule.Id,
            specialInformation?.Title ?? string.Empty,
            specialInformation?.Description ?? string.Empty,
            specialInformation?.HasFile ?? false,
            schedule.StartDate,
            schedule.EndDate);
    }
}