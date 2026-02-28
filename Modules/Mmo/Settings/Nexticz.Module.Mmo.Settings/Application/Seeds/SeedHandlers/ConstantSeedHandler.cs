using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Constants;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Application.Constants;
using Nexticz.Module.Mmo.Settings.Application.Constants.Commands.CreateConstant;
using Nexticz.Module.Mmo.Settings.Application.Constants.Queries.GetConstants;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Seeds.SeedHandlers;

internal class ConstantSeedHandler(ISender sender, ILogger<ConstantSeedHandler> logger)
    : BaseSeedHandler<GetConstantsQuery, Constant>(sender, logger), ISeedRunner
{
    private readonly ISender _sender = sender;

    private static List<CreateConstantRequest> ConstantsRequests =>
    [
        new(ConstantNames.SpaceBetweenPackagingsOnWashingMachine, "100", ConstantTypeContract.Int32, "Mezera mezi obaly, které se dávají na myčku. Hodnota je v milimetrech."),
        new(ConstantNames.WashingMachineAdjustmentTime, "15", ConstantTypeContract.Int32, "Čas potřebný na seřízení myčky pro novou dávku kitů."),
        new(ConstantNames.ShiftSettings, """
                                         [
                                           {
                                             "Name": "Ranní",
                                             "DailyOrder": 1,
                                             "IsActive": true,
                                             "Schedule": {
                                               "StartTimeOnly": "06:00:00",
                                               "EndTimeOnly": "18:00:00"
                                             }
                                           },
                                           {
                                             "Name": "Odpolední",
                                             "DailyOrder": 2,
                                             "IsActive": true,
                                             "Schedule": {
                                               "StartTimeOnly": "18:00:00",
                                               "EndTimeOnly": "6:00:00"
                                             }
                                           }
                                         ]
                                         """, 
            ConstantTypeContract.String, "Nastavení jednotlivých směn."),
        new(ConstantNames.BreakSettings, """
                                         [
                                           {
                                             "StartTimeOnly": "09:00",
                                             "EndTimeOnly": "09:15"
                                           },
                                           {
                                             "StartTimeOnly": "12:45",
                                             "EndTimeOnly": "13:30"
                                           },
                                           {
                                             "StartTimeOnly": "15:00",
                                             "EndTimeOnly": "15:15"
                                           }
                                         ]
                                         """, 
            ConstantTypeContract.String, "Nastavení jednotlivých směn."),
        new(ConstantNames.HoursBeforeNextShiftShouldBeCreated, "6", ConstantTypeContract.Int32, "Počet hodin odkdy se bude vytvářen nová směna před koncem současné směny, která buď ještě nezačala nebo je neaktivní.")
    ];
    
    public async Task RunSeedAsync(CancellationToken cancellationToken)
    {
        if (!await EnsureNoExistingDataAsync(new GetConstantsQuery(new BaseFilteringParams()), cancellationToken))
            return;
        
        foreach (var mmoConstantRequest in ConstantsRequests)
        {
            var createMmoConstantCommand = 
                new CreateConstantCommand(
                    mmoConstantRequest.Key, 
                    mmoConstantRequest.Value, 
                    (ConstantType)mmoConstantRequest.ConstantType,
                    mmoConstantRequest.Description);
            await _sender.Send(createMmoConstantCommand, cancellationToken);
        }
    }
}