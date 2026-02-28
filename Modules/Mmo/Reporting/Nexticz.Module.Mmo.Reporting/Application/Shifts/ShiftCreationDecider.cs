using Nexticz.Module.Mmo.Reporting.Application.Shifts.Models;
using Nexticz.Module.Mmo.Reporting.Application.ShiftSettings;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts;

internal class ShiftCreationDecider
{
    /// <summary>
    /// New shift should be created in cases:
    /// if active:
    ///   - determine whether within hours for the next shift -> either pick current or next settings
    /// if not active:
    ///   - if not within hours to create a new shift, no creation
    ///   - if within hours to create new shift, create next shift
    /// </summary>
    public static ShiftCreationDecisionResult ShouldCreateNewShift(Shift? lastShift, ShiftSettingManager shiftSettingManager, DateTimeOffset tenantNowOffset)
    {
        // this is the first shift
        if (lastShift is null)
            return new ShiftCreationDecisionResult(true, shiftSettingManager.CurrentShift.Name, shiftSettingManager.GetCurrentShiftSchedule());
        
        if (lastShift.Schedule.End > tenantNowOffset)
            return new ShiftCreationDecisionResult(false, null, null);
        
        var isWithinEndThreshold = shiftSettingManager.IsWithinEndThreshold();
        
        if (shiftSettingManager.CurrentShift.IsActive)
        {
            var nextShiftSettings = isWithinEndThreshold
                ? shiftSettingManager.GetNextShiftSetting() 
                : shiftSettingManager.CurrentShift;

            var nextShiftSchedule = isWithinEndThreshold
                ? shiftSettingManager.GetNextShiftSchedule() 
                : shiftSettingManager.GetCurrentShiftSchedule();
            
            return new ShiftCreationDecisionResult(true, nextShiftSettings.Name, nextShiftSchedule);
        }
        
        if (!isWithinEndThreshold)
            return new ShiftCreationDecisionResult(false, null, null);
        
        return new ShiftCreationDecisionResult(true, shiftSettingManager.GetNextShiftSetting().Name, shiftSettingManager.GetNextShiftSchedule());
    }
    
    
}