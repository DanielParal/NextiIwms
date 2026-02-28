using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Notifications;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines.Commands.CreateWashingMachine;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachines;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Seeds.SeedHandlers;

internal class WashingMachineSeedHandler(
    ISender sender, 
    ILogger<ManufactureSeedHandler> logger,
    ISettingsNotificationCollector notificationCollector)
    : BaseSeedHandler<GetWashingMachinesQuery, WashingMachine>(sender, logger), ISeedRunner
{
    private readonly ISender _sender = sender;

    private static List<CreateWashingMachineRequest> CreateWashingMachineRequests =>
    [
        new("MYCKA_1", null, WashingMachineStatusContract.Working, 25000, 30, 280, 420, 80,
            40, 2, 50, 65, 83),
        new("MYCKA_2", null, WashingMachineStatusContract.Working, 25000, 30, 250, 600, 80,
            40, 2, 42, 56, 108),
        new("MYCKA_3", null, WashingMachineStatusContract.Working, 25000, 30, 350, 1000, 80,
            40, 1, 46, 55, 59),
        new("MYCKA_4", null, WashingMachineStatusContract.Working, 25000, 30, 300, 420, 80,
            40, 2, 47, 54, 60),
        new("MYCKA_5", null, WashingMachineStatusContract.Working, 25000, 30, 330, 420, 80,
            40, 2, 46, 63, 78),
        new("MYCKA_6", null, WashingMachineStatusContract.Working, 25000, 30, 320, 400, 80,
            40, 1, 45, 55, 76),
        new("MYCKA_7", null, WashingMachineStatusContract.Working, 25000, 30, 400, 1000, 80,
            40, 2, 50, 75, 100)
    ];

    public async Task RunSeedAsync(CancellationToken cancellationToken)
    {
        if (!await EnsureNoExistingDataAsync(new GetWashingMachinesQuery(new BaseFilteringParams()), cancellationToken))
        {
            logger.LogInformation("Already existing washing machines in Settings. Publishing notification to planning to create washing machines in planning.");
            foreach (var createWashingMachineRequest in CreateWashingMachineRequests)
            {
                PublishNotificationToCreateWashingMachineInPlanning(createWashingMachineRequest);
            }
            
            return;
        }
        
        foreach (var createWashingMachineRequest in CreateWashingMachineRequests)
        {
            var createCommand = new CreateWashingMachineCommand(createWashingMachineRequest);
            await _sender.Send(createCommand, cancellationToken);
        }
    }

    private void PublishNotificationToCreateWashingMachineInPlanning(CreateWashingMachineRequest request)
    {
        var washingMachineLines = CreateWashingMachineLines(request.NumberOfLines, request.Code);
        
        var notification = new WashingMachineCreatedNotification(
            request.Code, request.Status,
            washingMachineLines.Select(x => 
                    new WashingMachineLineContract(x.Code, x.IsActive, new PrinterSettingsContract(null, null, null, null, null)))
                .ToArray());
        notificationCollector.AddNotification(notification);
    }
    
    private static WashingMachineLine[] CreateWashingMachineLines(int numberOfLines, string washingMachineCode)
    {
        var washingMachineLines = new WashingMachineLine[numberOfLines];
        for (var i = 0; i < numberOfLines; i++)
        {
            var code = $"{washingMachineCode}_L{1+i}";
            washingMachineLines[i] = new WashingMachineLine(code, true, new PrinterSettings(null, null, null, null, null));
        }
        
        return washingMachineLines;
    }
}