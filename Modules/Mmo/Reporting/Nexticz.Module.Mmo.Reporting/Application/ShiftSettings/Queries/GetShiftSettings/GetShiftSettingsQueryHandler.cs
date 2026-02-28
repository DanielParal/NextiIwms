using System.Text.Json;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Constants.Queries;
using Nexticz.Module.Mmo.Reporting.Application.Shifts;

namespace Nexticz.Module.Mmo.Reporting.Application.ShiftSettings.Queries.GetShiftSettings;

internal class GetShiftSettingsQueryHandler(
    ISender sender,
    ILogger<GetShiftSettingsQueryHandler> logger) : IRequestHandler<GetShiftSettingsQuery, ErrorOr<ShiftSetting[]>>
{
    public async Task<ErrorOr<ShiftSetting[]>> Handle(GetShiftSettingsQuery request, CancellationToken cancellationToken)
    {
        var shiftSettingsString = await sender.Send(
            new GetShiftSettingsConstantValueQuery(), cancellationToken);

        if (string.IsNullOrWhiteSpace(shiftSettingsString))
        {
            logger.LogError("Reporting - Shift settings is not set.");
            return ShiftErrors.ShiftSettingsNotFound;
        }
        
        ShiftSetting[]? shiftSettings;
        try
        {
            shiftSettings = JsonSerializer.Deserialize<ShiftSetting[]>(shiftSettingsString);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to deserialize shift settings. Error: {ErrorMessage}", ex.Message);
            return ShiftErrors.FailureDeserializeShiftSettings;
        }
        
        if (shiftSettings is null)
        {
            logger.LogError("Reporting - Shift settings is null.");
            return ShiftErrors.ShiftSettingsNotFound;
        }

        if (!ShiftSettingValidator.ValidateShifts(shiftSettings))
        {
            logger.LogError("Reporting - Shift settings is not valid.");
            return ShiftErrors.FailureInvalidShiftSettings;
        }
        
        return shiftSettings;
    }
}