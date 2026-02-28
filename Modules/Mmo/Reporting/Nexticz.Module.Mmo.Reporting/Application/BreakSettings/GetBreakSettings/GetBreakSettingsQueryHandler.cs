using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Constants.Queries;

namespace Nexticz.Module.Mmo.Reporting.Application.BreakSettings.GetBreakSettings;

internal class GetBreakSettingsQueryHandler(
    ISender sender,
    ILogger<GetBreakSettingsQueryHandler> logger) : IRequestHandler<GetBreakSettingsQuery, BreakSetting[]>
{
    public async Task<BreakSetting[]> Handle(GetBreakSettingsQuery request, CancellationToken cancellationToken)
    {
        var breakSettingsString = await sender.Send(new GetBreakSettingsConstantValueQuery(), cancellationToken);
        
        if (string.IsNullOrWhiteSpace(breakSettingsString))
        {
            logger.LogWarning("Reporting - Break settings is not set.");
            return [];
        }
        
        BreakSetting[]? breakSettings;
        try
        {
            breakSettings = JsonSerializer.Deserialize<BreakSetting[]>(breakSettingsString);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to deserialize break settings. Error: {ErrorMessage}", ex.Message);
            return [];
        }
        
        if (breakSettings is null)
        {
            logger.LogError("Reporting - break settings is null.");
            return [];
        }
        
        return breakSettings;
    }
}