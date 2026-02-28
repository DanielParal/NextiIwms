using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings.Queries;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetFilteredWashingMachineResponsesByPackagingCode;

internal class GetFilteredWashingMachineResponsesByPackagingCodeQueryHandler(
    ISender sender) : IRequestHandler<GetFilteredWashingMachineResponsesByPackagingCodeQuery, WashingMachineResponse[]>
{
    public async Task<WashingMachineResponse[]> Handle(GetFilteredWashingMachineResponsesByPackagingCodeQuery request,
        CancellationToken cancellationToken)
    {
        var packagingFromSettings =
            await sender.Send(new GetPackagingResponseByCodeQuery(request.PackagingCode), cancellationToken);

        if (packagingFromSettings.IsError)
            return [];

        var washingMachineFromSettings = 
            await sender.Send(new GetWashingMachineResponsesQuery(), cancellationToken);

        var filteredWashingMachinesFromSettings =
            FilterWashingMachines(washingMachineFromSettings, packagingFromSettings.Value);

        if (request.SisterPackagingCode is null)
            return filteredWashingMachinesFromSettings;

        var sisterPackagingFromSettings =
            await sender.Send(new GetPackagingResponseByCodeQuery(request.SisterPackagingCode), cancellationToken);

        if (sisterPackagingFromSettings.IsError)
            return [];
        
        var twoLinesWashingMachines = FilterTwoLinesWashingMachines(filteredWashingMachinesFromSettings);
        
        var washingMachineCodesFromSisterPackaging =
            FilterWashingMachines(twoLinesWashingMachines, sisterPackagingFromSettings.Value);

        var washingMachineResponses = filteredWashingMachinesFromSettings
            .Intersect(washingMachineCodesFromSisterPackaging)
            .OrderBy(x => x.Code)
            .ToArray();

        return washingMachineResponses;
    }
    
    private static WashingMachineResponse[] FilterTwoLinesWashingMachines(WashingMachineResponse[] washingMachines)
    {
        return washingMachines.Where(x => x.NumberOfLines == 2).ToArray();
    }

    private static WashingMachineResponse[] FilterWashingMachines(WashingMachineResponse[] washingMachines, PackagingResponse packaging)
    {
        // 1. filter only working washing machines
        var workingWashingMachines = 
            washingMachines
                .Where(x => x.Status == WashingMachineStatusContract.Working);

        // 2. first filter those lines which has set speeds in packaging 
        var filteredWashingMachinesBySetSpeeds =
            workingWashingMachines
                .Where(x =>
                    packaging.WashingMachineSpeeds.Any(p => p.WashingMachineCode == x.Code && p.Speed != WashingMachineSpeedLevelContract.NotSet));

        // 3. then filter those lines which has correct dimensions
        // filter by: Packaging Height dimension is Washing Machine Width
        // filter by: Packaging Width dimension is Washing Machine Height
        var filteredWashingMachinesByDimensions =
            filteredWashingMachinesBySetSpeeds
                .Where(x =>
                    x.MinWidth <= packaging.Dimensions.Height &&
                    x.MaxWidth >= packaging.Dimensions.Height &&
                    x.MaxHeight >= packaging.Dimensions.Width)
                .ToArray();

        return filteredWashingMachinesByDimensions.ToArray();
    }
}