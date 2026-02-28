using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Constants.Queries;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Reporting.Application.ShiftSettings.Queries.GetShiftSettings;

namespace Nexticz.Module.Mmo.Reporting.Application.ShiftSettings;

internal class ShiftSettingManagerFactory(ISender sender, ILogger<ShiftSettingManager> logger, IClock clock)
{
    public async Task<ShiftSettingManager> CreateAsync(CancellationToken cancellationToken)
    {
        var shiftSettings = await sender.Send(new GetShiftSettingsQuery(), cancellationToken);
        if (shiftSettings.IsError)
        {
            const string errorMessage = "MMO - Reporting - Shift settings are not set. We cannot create shift setting manager factory.";
            logger.LogError(errorMessage);
            throw new Exception(errorMessage);
        }
        
        var hoursBeforeNextShiftShouldBeCreated  = await sender.Send(new GetHoursBeforeNextShiftShouldBeCreatedConstantValueQuery(), cancellationToken);
        
        
        return ShiftSettingManager.Create(clock.TenantNowOffset.LocalDateTime, hoursBeforeNextShiftShouldBeCreated, shiftSettings.Value);
    }
}