using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Constants.Queries;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Module.Mmo.Settings.Contracts.Kits.Queries;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings.Queries;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Planning.Application.WashingCalculators;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetFilteredWashingMachineResponsesByPackagingCode;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetWashingMachineByLineQueueCode;
using Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateBatches;

internal class CreateBatchesBaseCommandHandler(
    ISender sender,
    ILogger<CreateBatchesBaseCommandHandler> logger
    ) : ICreateBatchesBaseCommandHandler
{
    public async Task<ErrorOr<KitResponse>> GetKitResponseByCodeAsync(string kitCode, CancellationToken cancellationToken)
    {
        var kitFromSettings = await sender.Send(new GetKitResponseByCodeQuery(kitCode), cancellationToken);

        if (kitFromSettings.IsError)
        {
            logger.LogWarning("Object {ObjectName} with code: {Code} does not exist. We cannot add batch to queue.",
                nameof(KitResponse), kitCode);
            return WashingMachineErrors.ValidationKitWithCodeDoesNotExistInSettings;
        }
        
        return kitFromSettings;
    }
    
    public async Task<ErrorOr<PackagingResponse>> GetPackagingResponseByCodeAsync(string packagingCode, CancellationToken cancellationToken)
    {
        var packagingFromSettings = await sender.Send(new GetPackagingResponseByCodeQuery(packagingCode), cancellationToken);
        
        if (packagingFromSettings.IsError)
        {
            logger.LogWarning("Object {ObjectName} with code: {Code} does not exist. We cannot add batch to queue.",
                nameof(PackagingResponse), packagingCode);
            return WashingMachineErrors.ValidationPackagingWithCodeDoesNotExistInSettings;
        }
        
        return packagingFromSettings;
    }

    public ErrorOr<Success> IsPackagingInKit(PackagingResponse packagingFromSettings, KitResponse kitFromSettings)
    {
        if (kitFromSettings.PackagingQuantities
            .All(x => x.PackagingCode != packagingFromSettings.Code))
        {
            logger.LogWarning("Packaging with code: {Code} is not in list of packagings for kit with code: {KitCode}. We cannot add batch to queue.",
                packagingFromSettings.Code, kitFromSettings.Code);
            return WashingMachineErrors.ValidationPackagingIsNotListedInKitPackagings;
        }

        return Result.Success;
    }

    public async Task<WashingMachineResponse[]> GetFilteredWashingMachineResponsesByPackagingCodeAsync(string packagingCode,
        string? sisterPackagingCode, CancellationToken cancellationToken)
    {
        return await sender.Send(
                new GetFilteredWashingMachineResponsesByPackagingCodeQuery(
                    packagingCode, sisterPackagingCode), cancellationToken);
    }
    
    public async Task<ErrorOr<WashingMachine>> GetWashingMachineByLineQueueCodeAsync(
        string lineQueueCode, WashingMachineResponse[] filteredWashingMachineResponses, CancellationToken cancellationToken)
    {
        var washingMachine = await sender.Send(new GetWashingMachineByLineQueueCodeQuery(lineQueueCode), cancellationToken);
        if (washingMachine.IsError)
        {
            logger.LogWarning("Object {ObjectName} with code: {Code} does not exist. We cannot add batch to queue.",
                nameof(LineQueue), lineQueueCode);
            return WashingMachineErrors.ValidationLineQueueDoesNotExist;
        }

        if (filteredWashingMachineResponses.All(x => x.Code != washingMachine.Value.Code))
        {
            logger.LogWarning(
                "Washing machine with code: {Code} is not in list of filtered washing machines. Filtered washing machine codes: {FilteredCodes} We cannot add batch to queue.",
                washingMachine.Value.Code, string.Join(", ", filteredWashingMachineResponses.Select(x => x.Code)));
            return WashingMachineErrors.ValidationWashingMachineIsNotPresentedInFilteredWashingMachines;
        }
        
        return washingMachine.Value;
    }

    public async Task<int> GetSpaceBetweenPackagingsConstantAsync(CancellationToken cancellationToken)
    {
        return await sender.Send(
            new GetSpaceBetweenPackagingsOnWashingMachineConstantValueQuery(), cancellationToken);
    }

    public ErrorOr<TimeSpan> CalculateOptimalWashingTime(
        KitResponse kit,
        PackagingResponse packaging,
        WashingMachineResponse washingMachineResponse,
        int spaceBetweenPackagings)
    {
        var packagingWidth = packaging.Dimensions.Width;
        var packagingCount = kit.PackagingQuantities.First(x => x.PackagingCode == packaging.Code).Quantity;
        var packagingSpeed = packaging.WashingMachineSpeeds.First(x => x.WashingMachineCode == washingMachineResponse.Code).Speed;
        var washingMachineSpeed = GetWashingMachineSpeed(packagingSpeed, washingMachineResponse);
        if (washingMachineSpeed.IsError)
            return washingMachineSpeed.Errors;
        
        var optimalTimeDuration = WashingTimeCalculator.CalculateOptimalKitTime(
            packagingWidth, packagingCount, washingMachineSpeed.Value, spaceBetweenPackagings);
        
        if (optimalTimeDuration.IsError)
            return optimalTimeDuration.Errors;
        
        return optimalTimeDuration.Value;
    }

    private ErrorOr<int> GetWashingMachineSpeed(WashingMachineSpeedLevelContract speedLevel, WashingMachineResponse washingMachineResponse)
    {
        var propertyName = speedLevel.ToString();
        
        var speedProperty = washingMachineResponse.GetType().GetProperty(propertyName);
        if (speedProperty == null) 
        {
            logger.LogWarning(
                "Invalid speed level: {SpeedLevel} requested for washing machine with code: {Code}.",
                speedLevel, washingMachineResponse.Code);
            return WashingMachineErrors.ValidationInvalidSpeedLevel;
        }
        
        if (speedProperty.GetValue(washingMachineResponse) is int speedValue)
        {
            return speedValue;
        }
    
        logger.LogWarning(
            "Washing machine with code: {Code} does not contain a valid speed value for level {SpeedLevel}.",
            washingMachineResponse.Code, speedLevel);
        return WashingMachineErrors.ValidationInvalidSpeedLevel;
        
    }
}